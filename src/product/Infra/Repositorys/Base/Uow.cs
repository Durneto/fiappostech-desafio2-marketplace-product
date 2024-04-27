using System.Data;

namespace ProductApi.Infra.Repositorys.Base
{
    public class Uow : IUow
    {
        public IDbTransaction Transaction { get; private set; }
        public IDbConnection Connection { get; private set; }

        public Uow(IDbConnection connection)
        {
            Connection = connection;
        }

        public void Open()
        {
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
        }

        public void BeginTransaction()
        {
            if (Transaction == null)
                Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            Transaction?.Commit();
            Transaction = null;
        }

        public void Rollback()
        {
            Transaction?.Rollback();
            Transaction = null;
        }

        public void Dispose()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Transaction?.Dispose();
                Connection?.Close();
                Connection?.Dispose();
            }
        }
    }
}
