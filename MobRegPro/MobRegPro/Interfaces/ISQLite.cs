using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobRegPro
{
    public interface ISQLite
    {
        SQLiteConnection GetSQLiteConnection();
        bool DeleteDatabase();
        bool DatabaseExists();
    }

}
