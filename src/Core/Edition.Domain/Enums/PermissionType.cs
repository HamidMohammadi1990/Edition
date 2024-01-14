using System.ComponentModel.DataAnnotations;

namespace Edition.Domain.Enums;

public enum PermissionType : int
{
    [Display(Name = "الو چاپ")]
    Product = 1,

    [Display(Name = "مدیریت کاربران")]
    ManageUsersGroup = 2,

    [Display(Name = "مدیریت کاربران")]
    ManageUsers = 3,

    [Display(Name = "ثبت کاربران")]
    CreateUser = 4,

    [Display(Name = "ویرایش کاربران")]
    UpdateUser = 5,

    [Display(Name = "لیست کاربران")]
    ListUser = 6,

    [Display(Name = "حذف کاربران")]
    DeleteUser = 7,

    [Display(Name = "یافتن بر اساس شناسه")]
    GetUserById = 8,
}