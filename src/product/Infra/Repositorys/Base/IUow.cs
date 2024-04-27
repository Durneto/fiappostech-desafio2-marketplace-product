using System.Data;

namespace ProductApi.Infra.Repositorys.Base
{
    public interface IUow : IDisposable
    {
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }
        void Open();
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
