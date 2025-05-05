namespace tao_project.Models.Process
{
    public enum SystemPermissions
    {
        // Employee
        EmployeeView, EmployeeCreate, EmployeeEdit, EmployeeDelete,
        
        // MemberUnit (đã sửa typo)
        MemberUnitView, MemberUnitCreate, MemberUnitEdit, MemberUnitDelete,
        
        // Role
        RoleView, RoleCreate, RoleEdit, RoleDelete, RoleAssignClaim,
        
        // Account
        AccountView, AssignRole, AddClaim, DeleteClaim
    }
}