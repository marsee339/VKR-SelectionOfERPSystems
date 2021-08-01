using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.SQLite;
using System.Data.SQLite.Linq;

namespace SelectionOfERPSystems
{    
    class ERPDBContext : DbContext
    {        
        static string DataBaseConnectionString = "../../Data/db.sqlite3";
        public DbSet<Обозначение> Обозначения { get; set; }
        public DbSet<Отрасль> Отрасли { get; set; }
        public DbSet<Компания> Компании { get; set; }
        public DbSet<Принадлежность_продукта> Принадлежности_продуктов { get; set; }
        public DbSet<Продукт> Продукты { get; set; }
        public ERPDBContext() : base(CreateConnection(DataBaseConnectionString), false){ } 
        static SQLiteConnection CreateConnection(string path)
        {
            var builder = (SQLiteConnectionStringBuilder)SQLiteProviderFactory.Instance.CreateConnectionStringBuilder();
            if (builder == null) return null;
            builder.DataSource = path;
            builder.FailIfMissing = false;
            return new SQLiteConnection(builder.ToString());
        }
    }
    public class Обозначение
    {
        [Key]
        public int номер { get; private set; }
        public string значение { get; private set; }
    }
    public class Отрасль
    {
        [Key]
        public int номер { get; private set; }
        public string название { get; private set; }
    }
    public class Компания
    {
        [Key]
        public int номер { get; private set; }
        public string название { get; private set; }
        public string сайт { get; private set; }
        public string страна { get; private set; }
        public int год_основания { get; private set; }
        public int модульность { get; private set; }
        public string пробный_период_в_днях { get; private set; }
        public int учёт_законодательства_РФ { get; private set; }
        public string информация { get; private set; }
    }
    public class Принадлежность_продукта
    {
        [Key, Column(Order = 1)]
        public int продукт { get; private set; }
        [Key, Column(Order = 2)]
        public int компания { get; private set; }
        [Key, Column(Order = 3)]
        public int отрасль { get; private set; }
    }
    public class Продукт
    {
        [Key]
        public int номер { get; private set; }
        public string название { get; private set; }
        public string сайт { get; private set; }
        public int размер_компании { get; private set; }
        public int развёртывание { get; private set; }
        public int? интеграция1 { get; private set; }
        public int? интеграция2 { get; private set; }
        public int? интеграция3 { get; private set; }
        public int? интеграция4 { get; private set; }
        [Column("maxСтоимость_1модуль/мес_руб")]
        public int? cost_1modul_month { get; private set; }
        [Column("maxСтоимость_1чел/мес_руб")]
        public int? cost_1person_month { get; private set; }
        [Column("maxСтоимость_за1/навсегда_руб")]
        public int? cost_1_forever { get; private set; }
        [Column("maxСтоимость_навсегда_руб")]
        public int? cost_forever { get; private set; }
        public int управление_активами { get; private set; }
        public int финансы { get; private set; }
        public int управление_производством { get; private set; }
        public int управление_продажами { get; private set; }
        public int управление_обслуживанием { get; private set; }
        public int выбор_поставщиков { get; private set; }
        public int цепочки_поставок { get; private set; }
        public int взаимоотношения_с_клиентами { get; private set; }
        public int закупки_запасы { get; private set; }
        public int аналитика_отчётность { get; private set; }
        public int мобильные_технологии { get; private set; }
        public int управление_персоналом { get; private set; }
        public int управление_проектами { get; private set; }
        public int дистрибуция { get; private set; }
        public int управление_эффективностью { get; private set; }
        public int управление_ЖЦ { get; private set; }
        public int госзакупки_гособоронзаказ { get; private set; }
        public int обслуживание_оборудования { get; private set; }
        public int мультиатрибутная_продукция { get; private set; }
        public int обеспечение_деятельности { get; private set; }
        public int маркетинг { get; private set; }
        public int планирование_графики { get; private set; }
        public int экология_безопасность { get; private set; }
        public int логистика_транспортировка { get; private set; }
        public int мерчандайзинг { get; private set; }
        public int контроль_качества { get; private set; }
        public int портал_с_ЛК { get; private set; }
        public int взаимоотношения_с_контрагентами { get; private set; }
    }
}
