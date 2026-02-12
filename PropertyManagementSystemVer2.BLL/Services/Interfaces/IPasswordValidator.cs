namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IPasswordValidator
    {
        List<string> Validate(string password);
        bool IsStrong(string password);
    }
}
