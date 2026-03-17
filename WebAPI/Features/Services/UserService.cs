using General.Common.Result;
using General.Common.Services;
using General.Data;
using General.Data.Types;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Features.Services;

public class UserService(AppDbContext context, ILogger<UserService> logger) : CrudService<User>(context, logger)
{
    public override Task<ProcessResult> Create(User data, CancellationToken cancellationToken)
    {
        data.Password = (new PasswordHasher<User>()).HashPassword(data, data.Password);
        return base.Create(data, cancellationToken);
    }

    public override Task<ProcessResult> CreateBatch(IEnumerable<User> datas, CancellationToken cancellationToken)
    {
        var enumerable = datas.ToList();
        
        foreach (var data in enumerable) 
            data.Password = (new PasswordHasher<User>()).HashPassword(data, data.Password);
        
        return base.CreateBatch(enumerable, cancellationToken);
    }

    public override Task<ProcessResult> Update(User inputData, CancellationToken cancellationToken)
    {
        inputData.Password = (new PasswordHasher<User>()).HashPassword(inputData, inputData.Password);
        return base.Update(inputData, cancellationToken);
    }

    public override Task<ProcessResult> UpdateBatch(IEnumerable<User> inputDatas, CancellationToken cancellationToken)
    {
        var enumerable = inputDatas.ToList();
        
        foreach (var data in enumerable) 
            data.Password = (new PasswordHasher<User>()).HashPassword(data, data.Password);
        
        return base.UpdateBatch(enumerable, cancellationToken);
    }
}