using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using tao_project.Models;
using tao_project.Data;
using Microsoft.EntityFrameworkCore;
using tao_project.Models.Process;
using System.Data;
using OfficeOpenXml;
using OfficeOpenXml.Style;  // Quan trọng cho ExcelFillStyle và ExcelHorizontalAlignment
using System.Drawing;       // Quan trọng cho Color
using X.PagedList;


namespace tao_project.Controllers
{
    public class PersonController : Controller
    {
       private readonly ApplicationDbContext _context;
       private ExcelProcess _excelProcess = new ExcelProcess();
       public PersonController(ApplicationDbContext context)
       {
           _context = context;
       }


       public async Task<IActionResult> Index()
       {
           var persons = await _context.Persons.ToListAsync();
           return View(persons);
       }


       public IActionResult Create()
       {
          return View();
       }
       [HttpPost]
       [ValidateAntiForgeryToken]
         public async Task<IActionResult> Create([Bind("Id,Fullname,Address")] Person person)
         {
              if (ModelState.IsValid)
              {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
              }
              return View(person);
         }
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(string id, [Bind("Id,Fullname,Address")] Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }
       public  async Task<IActionResult> Delete(string id)
       {
           if (id == null || _context.Persons == null)
           {
               return NotFound();
           }

           var person = await _context.Persons
               .FirstOrDefaultAsync(m => m.Id == id);
           if (person == null)
           {
               return NotFound();
           }

           return View(person);
       }
       [HttpPost, ActionName("Delete")]
       [ValidateAntiForgeryToken]
       public async Task<IActionResult> DeleteConfirmed(string id)
       {
           if (_context.Persons == null)
           {
               return Problem("Entity set 'ApplicationDbContext.Persons'  is null.");
           }
           var person = await _context.Persons.FindAsync(id);
           if (person != null)
           {
               _context.Persons.Remove(person);
           }

           await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
       }
       public bool PersonExists(string id)
       {
           return (_context.Persons?.Any(e => e.Id == id)).GetValueOrDefault();
       }

       public async Task<IActionResult>  Upload()
       {
        return View();
       }
    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Upload(IFormFile file)
{
     
    
    if (file == null || file.Length == 0)
    {
        ModelState.AddModelError("", "Vui lòng chọn file");
        return View();
    }

    string fileExtension = Path.GetExtension(file.FileName).ToLower();
    if (fileExtension != ".xls" && fileExtension != ".xlsx")
    {
        ModelState.AddModelError("", "Chỉ hỗ trợ định dạng .xls và .xlsx");
        return View();
    }

    try
    {
        // Tạo thư mục nếu chưa tồn tại
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "excels");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // Tạo tên file unique
        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Đọc file Excel
        var dt = _excelProcess.ExcelToDataTable(filePath);
        
        // Bắt transaction
        using (var transaction = await _context.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    // Kiểm tra số lượng cột
                    if (row.ItemArray.Length < 3)
                    {
                        continue; // Bỏ qua nếu không đủ 3 cột
                    }

                    var ps = new Person
                    {
                        Id = row[0]?.ToString()?.Trim(),
                        Fullname = row[1]?.ToString()?.Trim(),
                        Address = row[2]?.ToString()?.Trim()
                    };

                    // Kiểm tra dữ liệu bắt buộc
                    if (string.IsNullOrEmpty(ps.Id))
                    {
                        continue;
                    }

                    // Kiểm tra trùng ID
                    if (await _context.Persons.AnyAsync(p => p.Id == ps.Id))
                    {
                        continue; // hoặc có thể thêm thông báo lỗi
                    }

                    _context.Persons.Add(ps);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                TempData["Message"] = $"Import thành công {dt.Rows.Count} bản ghi";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", $"Lỗi khi xử lý dữ liệu: {ex.Message}");
                return View();
            }
        }
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", $"Lỗi khi import file: {ex.Message}");
        return View();
    }
      }

      public IActionResult Download()
{
    var fileName = "DanhSachPerson_" + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";
    
    using (ExcelPackage excelPackage = new ExcelPackage())
    {
        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("DanhSach");
        
        // Tiêu đề các cột
        worksheet.Cells["A1"].Value = "Mã Person";
        worksheet.Cells["B1"].Value = "Họ và Tên";
        worksheet.Cells["C1"].Value = "Địa Chỉ";
        
        var danhSachPerson = _context.Persons.ToList();
        
        if (danhSachPerson.Any())
        {
            worksheet.Cells["A2"].LoadFromCollection(danhSachPerson);
        }
        
        // Định dạng hàng tiêu đề (đã sửa lỗi chính tả)
        using (var range = worksheet.Cells["A1:C1"])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;  // Đã sửa từ ExcellEllStyle
            range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue);  // Sử dụng System.Drawing.Color
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;  // Đã sửa từ ExcellHorizontalAlignment
        }
        
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        
        var stream = new MemoryStream(excelPackage.GetAsByteArray());
        return File(stream, 
                  "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                  fileName);
    }
}
    }

    }