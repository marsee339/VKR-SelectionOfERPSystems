using System.Collections.Generic;

namespace SelectionOfERPSystems
{
    public class ColumnToModuleName
    {
        public string ColumnName { get; private set; }
        public string Name { get; private set; }
        public ColumnToModuleName(string name, string columnName)
        {
            Name = name;
            ColumnName = columnName;
        }
        public override string ToString()
        {
            return Name;
        }
        public static List<ColumnToModuleName> GetColumtNameMappings()
        {
            var nameMapping = new List<ColumnToModuleName>()
            {
                new ColumnToModuleName("Управление активами (EAM)", "управление_активами"),
                new ColumnToModuleName("Финансы (бухгалтерия и казначейство)", "финансы"),
                new ColumnToModuleName("Управление производством (MES)", "управление_производством"),
                new ColumnToModuleName("Управление продажами", "управление_продажами"),
                new ColumnToModuleName("Управление обслуживанием (ESM) ", "управление_обслуживанием"),
                new ColumnToModuleName("Выбор поставщиков ", "выбор_поставщиков"),
                new ColumnToModuleName("Цепочки поставок (Supply Chain)", "цепочки_поставок"),
                new ColumnToModuleName("Управление взаимоотношениями с клиентами (CRM и CEM)", "взаимоотношения_с_клиентами"),
                new ColumnToModuleName("Закупки и контроль запасов", "закупки_запасы"),
                new ColumnToModuleName("Аналитика и отчётность (документооборот)", "аналитика_отчётность"),
                new ColumnToModuleName("Использование мобильных технологий", "мобильные_технологии"),
                new ColumnToModuleName("Управление персоналом (HCM)", "управление_персоналом"),
                new ColumnToModuleName("Управление проектами", "управление_проектами"),
                new ColumnToModuleName("Дистрибуция", "дистрибуция"),
                new ColumnToModuleName("Управление эффективностью организации (EPM)", "управление_эффективностью"),
                new ColumnToModuleName("Управление ЖЦ", "управление_ЖЦ"),
                new ColumnToModuleName("Госзакупки / Гособоронзаказ", "госзакупки_гособоронзаказ"),
                new ColumnToModuleName("Аренда и/или обслуживание оборудования (автотранспорта)", "обслуживание_оборудования"),
                new ColumnToModuleName("Управление мультиатрибутной продукцией", "мультиатрибутная_продукция"),
                new ColumnToModuleName("Обеспечение деятельности", "обеспечение_деятельности"),
                new ColumnToModuleName("Маркетинг", "маркетинг"),
                new ColumnToModuleName("Планирование и составление графиков", "планирование_графики"),
                new ColumnToModuleName("Экологические требования, охрана труда и техника безопасности", "экология_безопасность"),
                new ColumnToModuleName("Логистика, транспортировка", "логистика_транспортировка"),
                new ColumnToModuleName("Мерчандайзинг", "мерчандайзинг"),
                new ColumnToModuleName("Контроль качества продукции", "контроль_качества"),
                new ColumnToModuleName("Портал с ЛК пользователей", "портал_с_ЛК"),
                new ColumnToModuleName("Взаимоотношения с контрагентами", "взаимоотношения_с_контрагентами"),
            };
            return nameMapping;
        }
    }    
}
