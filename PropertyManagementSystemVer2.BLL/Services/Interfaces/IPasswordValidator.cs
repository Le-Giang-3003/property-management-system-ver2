namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IPasswordValidator
    {
        bool IsStrong(string password);

    }
}
