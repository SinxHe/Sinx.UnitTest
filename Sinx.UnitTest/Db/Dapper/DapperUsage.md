# Dapper Usage
## Dapper 官方文档
关于Dapper的常用简单用法可以参考[官方文档](https://github.com/StackExchange/Dapper)
## Dapper 


## Dapper 中使用事务

### 封装的一个使用事务的方法
调用:  
注意Lambda表达式应该使用异步形式, 在内部加上`await`以保证异常能够被`TransactionWrapperAsync`方法捕获并进行回滚处理.
```csharp
var entities = await _db.QueryAsync<TEntity>(spName, new { Id = id }, commandType: CommandType.StoredProcedure);
```
```csharp
protected async Task<TResult> TransactionWrapperAsync<TResult>(Func<Task<TResult>> func)
{
    _db.Open();
    var tran = _db.BeginTransaction();
    TResult r;
    try
    {
        r = await func();
        tran.Commit();
    }
    catch (Exception)
    {
        tran.Rollback();
        throw;
    }
    finally
    {
        _db.Close();
    }
    return r;
}
```