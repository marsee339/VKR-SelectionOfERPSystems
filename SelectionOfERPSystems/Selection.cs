using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SelectionOfERPSystems
{
    class Selection
    {
        bool remove = true;
        List<Product> products;
        string industy { get; }        
        string size { get; }
        string deploy { get; }
        int RFLaws { get; }
        List<ColumnToModuleName> modules { get; }
        List<string> integration { get; }
        int costMonth { get; }
        int costForever { get; }
        int workers { get; }
        int modular { get; }
        int trial { get; }
        string fSize { get; }
        List<ColumnToModuleName> fModules { get; }
        class Product
        {
            public IQueryable<Продукт> info;
            public string marks;
            public int marksPlus;
            public Product(IQueryable<Продукт> info, string marks, int marksPlus=0)
            {
                this.info = info;
                this.marks = marks;
                this.marksPlus = marksPlus;
            }
        }
        struct PairKeyValue 
        {
            public int key;
            public int value;
            public PairKeyValue(int key, int value)
            {
                this.key = key;
                this.value = value;
            }
        }
        public Selection(string industy, string size, string deploy, int RFLaws, List<ColumnToModuleName> modules, List<string> integration, int costMonth, int costForever, int workers, int modular, int trial, string fSize, List<ColumnToModuleName> fModules)
        {
            this.industy = industy;
            this.size = size;
            this.deploy = deploy;
            this.RFLaws = RFLaws;
            this.modules = modules;
            this.integration = integration;
            this.costMonth = costMonth;
            this.costForever = costForever;
            this.workers = workers;
            this.modular = modular;
            this.trial = trial;
            this.fSize = fSize;
            this.fModules = fModules;
        }
        void ProductsRemove()
        {
            var values = new List<PairKeyValue>();
            for (int i = 0; i < products.Count(); i++)
                values.Add(new PairKeyValue(i, products[i].marksPlus));
            values = values.OrderByDescending(pair => pair.value).ToList();

            List<Product> newProducts = new List<Product>();
            for (int i = 2; i < values.Count; i++)
            {
                if (values[i].value != values[i - 1].value)
                {
                    for (int j = 0; j < i; j++)
                    {
                        int productId = values[j].key;
                        newProducts.Add(new Product(products[productId].info, products[productId].marks, products[productId].marksPlus));
                    }
                    break;
                }
            }
            if (newProducts.Count!=0)
                products = newProducts;

            if (products.Count() <= 2)
                remove = false;
        }
        string ToOutputString(ERPDBContext DB)
        {
            string output = $"ERP-системы отрасли {industy}:\n";
            int resProductsCount = products.Count();

            int valAll = products[0].marksPlus; //всего рассмотрено параметров
            foreach (var mark in products[0].marks)
                if (mark == '-')
                    valAll++;

            var values = new List<PairKeyValue>(); //sort
            for (int i = 0; i < products.Count(); i++)
                values.Add(new PairKeyValue(i, products[i].marksPlus));
            values = values.OrderByDescending(pair => pair.value).ToList();

            if (resProductsCount > 0)
            {
                for (int i = 0; i < resProductsCount; i++)
                    output += $"{values[i].key + 1}) {products[values[i].key].info.Select(j => j.название).First()} [{values[i].value}/{valAll}]\n";
                output += "_________________________________________________________________________\n";
                for (int k = 0; k < resProductsCount; k++)
                {
                    int j = values[k].key;
                    output += $"\t\t{products[j].info.Select(i => i.название).First()}\n";
                    int productID = products[j].info.Select(i => i.номер).First();
                    int companyID = DB.Принадлежности_продуктов.Where(i => i.продукт == productID).Select(i => i.компания).First();
                    var company = DB.Компании.Where(i => i.номер == companyID);
                    output += $"[Компания] \n{company.Select(i => i.название).First()}, год основания - {company.Select(i => i.год_основания).First()}, {company.Select(i => i.страна).First()}\n" +
                        $"Сайт: {company.Select(i => i.сайт).First()}\n";
                    if (company.Select(i => i.информация).First() != null)
                        output += $"Дополнительная информация: {company.Select(i => i.информация).First()}\n";
                    output += $"[Продукт] \nСайт: {products[j].info.Select(i => i.сайт).First()}\n";
                    output += products[j].marks;
                    if (output[output.Length - 2] == '\n') //удаление лишнего переноса в конце информации
                        output = output.Remove(output.Length - 1, 1);
                    output += "_________________________________________________________________________\n";
                }
            }
            else
                output += "Не подобраны (а должны!)";
            return output;
        }        
        void FModulesSelect(ERPDBContext DB)
        {
            for (int i = 0; i < products.Count(); i++)
            {
                products[i].marks += "Модули в перспективе:\n";
                for (int j = 0; j < fModules.Count; j++)
                {
                    ColumnToModuleName c = (ColumnToModuleName)fModules[j];
                    int productNum = products[i].info.Select(p => p.номер).First();
                    var product = DB.Продукты.Where(m => m.номер == productNum).First();
                    int val = Convert.ToInt32(product.GetType().GetProperty(c.ColumnName).GetValue(product, null));
                    if (val == 1)
                    {
                        products[i].marks += $"+{fModules[j].Name}\n";
                        products[i].marksPlus++;
                    }
                    else
                        products[i].marks += $"-{fModules[j].Name}\n";
                }
                products[i].marks += "\n";
            }
            if (remove)
                ProductsRemove();
        }
        void FSizeSelect(ERPDBContext DB)
        {
            int[] fSizes = DB.Обозначения.ToArray().Where(o => o.значение.Contains(fSize)).Select(o => o.номер).ToArray(); //доступные номера размеров
            for (int i = 0; i < products.Count(); i++)
            {
                int productSizeNum = products[i].info.Select(p => p.размер_компании).First();
                string productSize = DB.Обозначения.Where(k => k.номер == productSizeNum).Select(k => k.значение).First();
                if (products[i].info.Where(p => fSizes.Contains(p.размер_компании)).FirstOrDefault() == null)
                    products[i].marks += $"-В перспективе подходит только предприятиям размера: {productSize}\n";
                else
                {
                    products[i].marks += $"+В перспективе подходит предприятиям размера: {productSize}\n";
                    products[i].marksPlus++;
                }
            }
            if (remove)
                ProductsRemove();
        }
        void CostSelect(ERPDBContext DB)
        {
            for (int i = 0; i < products.Count(); i++)
            {
                int productNum = products[i].info.Select(p => p.номер).First();
                int companyNum = DB.Принадлежности_продуктов.Where(с => с.продукт == productNum).Select(с => с.компания).First();
                int companyModular = DB.Компании.Where(c => c.номер == companyNum).Select(c => c.модульность).First();
                int? rent = 0;
                int? buy = 0;
                if (companyModular == 1)
                {
                    int modulesCount = modules.Count() == 0 ? 1 : modules.Count();
                    rent += modulesCount * (products[i].info.Select(c => c.cost_1modul_month).FirstOrDefault() == null ? 0 : products[i].info.Select(c => c.cost_1modul_month).First());
                    buy += modulesCount * (products[i].info.Select(c => c.cost_1_forever).FirstOrDefault() == null ? 0 : products[i].info.Select(c => c.cost_1_forever).First());
                }
                rent += workers * (products[i].info.Select(c => c.cost_1person_month).FirstOrDefault() == null ? 0 : products[i].info.Select(c => c.cost_1person_month).First());
                buy += workers * (products[i].info.Select(c => c.cost_1_forever).FirstOrDefault() == null ? 0 : products[i].info.Select(c => c.cost_1_forever).First());
                int? buySimple = products[i].info.Select(c => c.cost_forever).FirstOrDefault();
                if (buySimple != null)
                    buy += buySimple;

                if (rent > costMonth)
                    products[i].marks += $"-Ежемесячная аренда будет стоить около: {rent} руб.\n";
                else if (rent != 0)
                {
                    products[i].marks += $"+Ежемесячная аренда будет стоить около: {rent} руб.\n";
                    products[i].marksPlus++;
                }
                else
                    products[i].marks += $"-Отсутствуют данные о стоимости аренды\n";
                if (buy > costForever)
                    products[i].marks += $"-Покупка будет стоить около: {buy} руб.\n";
                else if (buy != 0)
                {
                    products[i].marks += $"+Покупка будет стоить около: {buy} руб.\n";
                    products[i].marksPlus++;
                }
                else
                    products[i].marks += $"-Отсутствуют данные о стоимости покупки\n";
            }
            if (remove)
                ProductsRemove();
        }
        void TrialSelect(ERPDBContext DB)
        {
            for (int i = 0; i < products.Count(); i++)
            {
                int productNum = products[i].info.Select(p => p.номер).First();
                int companyNum = DB.Принадлежности_продуктов.Where(с => с.продукт == productNum).Select(с => с.компания).First();
                string trialString = DB.Компании.Where(c => c.номер == companyNum).Select(c => c.пробный_период_в_днях).First();
                if (trialString == null)
                    products[i].marks += "-Нет пробного периода\n";
                else
                {
                    products[i].marks += $"+Пробный период (дней): {trialString}\n";
                    products[i].marksPlus++;
                }
            }
            if (remove)
                ProductsRemove();
        }
        void ModularSelect(ERPDBContext DB)
        {
            for (int i = 0; i < products.Count(); i++)
            {
                int productNum = products[i].info.Select(p => p.номер).First();
                int companyNum = DB.Принадлежности_продуктов.Where(с => с.продукт == productNum).Select(с => с.компания).First();
                if (DB.Компании.Where(c => c.номер == companyNum).Select(c => c.модульность).First() == 0)
                    products[i].marks += "-Не модульное распространение\n";
                else
                {
                    products[i].marks += "+Модульное распространение\n";
                    products[i].marksPlus++;
                }
            }
            if (remove)
                ProductsRemove();
        }
        void IntegrationSelect(ERPDBContext DB)
        {
            for (int i = 0; i < products.Count(); i++)
            {
                products[i].marks += "Доступные интеграции:\n";
                int productNum = products[i].info.Select(p => p.номер).First();
                List<string> productInteg = new List<string>();
                var integ1 = DB.Продукты.Where(m => m.номер == productNum).Select(m => m.интеграция1).FirstOrDefault();
                if (integ1 != null)
                {
                    productInteg.Add(DB.Обозначения.Where(o => o.номер == integ1).Select(o => o.значение).First());
                    var integ2 = DB.Продукты.Where(m => m.номер == productNum).Select(m => m.интеграция2).FirstOrDefault();
                    if (integ2 != null)
                    {
                        productInteg.Add(DB.Обозначения.Where(o => o.номер == integ2).Select(o => o.значение).First());
                        var integ3 = DB.Продукты.Where(m => m.номер == productNum).Select(m => m.интеграция3).FirstOrDefault();
                        if (integ3 != null)
                        {
                            productInteg.Add(DB.Обозначения.Where(o => o.номер == integ3).Select(o => o.значение).First());
                            var integ4 = DB.Продукты.Where(m => m.номер == productNum).Select(m => m.интеграция4).FirstOrDefault();
                            if (integ4 != null)
                            {
                                productInteg.Add(DB.Обозначения.Where(o => o.номер == integ4).Select(o => o.значение).First());
                            }
                        }
                    }
                }
                foreach (var integ in integration)
                {
                    if (productInteg.Contains(integ))
                    {
                        products[i].marks += $"+{integ}\n";
                        products[i].marksPlus++;
                    }
                    else
                        products[i].marks += $"-{integ}\n";
                }
                products[i].marks += "\n";
            }

            if (remove)
                ProductsRemove();
        }
        void ModulesSelect(ERPDBContext DB)
        {
            for (int i = 0; i < products.Count(); i++)
            {
                products[i].marks += "Модули:\n";
                for (int j = 0; j < modules.Count; j++)
                {
                    ColumnToModuleName c = (ColumnToModuleName)modules[j];
                    int productNum = products[i].info.Select(p => p.номер).First();
                    var product = DB.Продукты.Where(m => m.номер == productNum).First();
                    int val = Convert.ToInt32(product.GetType().GetProperty(c.ColumnName).GetValue(product, null));
                    if (val == 0)
                        products[i].marks += $"-{modules[j].Name}\n";
                    else
                    {
                        products[i].marks += $"+{modules[j].Name}\n";
                        products[i].marksPlus++;
                    }
                }
                products[i].marks += "\n";
            }

            if (remove)
                ProductsRemove();
        }
        void RFSelect(ERPDBContext DB)
        {
            for (int i = 0; i < products.Count(); i++)
            {
                int productNum = products[i].info.Select(p => p.номер).First();
                int companyNum = DB.Принадлежности_продуктов.Where(с => с.продукт == productNum).Select(с => с.компания).First();
                if (DB.Компании.Where(c => c.номер == companyNum).Select(c => c.учёт_законодательства_РФ).First() == 0)
                    products[i].marks += "-Не учитывает законадательство РФ\n";
                else
                {
                    products[i].marks += "+Учитывает законадательство РФ\n";
                    products[i].marksPlus++;
                }
            }
            if (remove)
                ProductsRemove();
        }
        void DeploySelect(ERPDBContext DB)
        {
            int[] deployNums = DB.Обозначения.ToArray().Where(o => o.значение.Contains(deploy)).Select(o => o.номер).ToArray(); //доступные номера развёртывания
            for (int i = 0; i < products.Count(); i++)
            {
                int productDeploy = products[i].info.Select(p => p.развёртывание).First();
                if (!deployNums.Contains(productDeploy))
                    products[i].marks += $"-Возможное развёртывание только: {DB.Обозначения.Where(d => d.номер == productDeploy).Select(d => d.значение).First()}\n";
                else
                {
                    products[i].marks += $"+Возможное развёртывание: {DB.Обозначения.Where(d => d.номер == productDeploy).Select(d => d.значение).First()}\n";
                    products[i].marksPlus++;
                }
            }
            if (remove)
                ProductsRemove();
        }
        void SizeSelect(ERPDBContext DB)
        {
            int[] sizes = DB.Обозначения.ToArray().Where(o => o.значение.Contains(size)).Select(o => o.номер).ToArray(); //доступные номера размеров
            for (int i = 0; i < products.Count(); i++) //подбор по размеру
            {
                int productSizeNum = products[i].info.Select(p => p.размер_компании).First();
                string productSize = DB.Обозначения.Where(k => k.номер == productSizeNum).Select(k => k.значение).First();
                if (products[i].info.Where(p => sizes.Contains(p.размер_компании)).FirstOrDefault() == null)
                    products[i].marks += $"-Только для предприятия размера: {productSize}\n";
                else
                {
                    products[i].marks += $"+Для предприятия размера: {productSize}\n";
                    products[i].marksPlus++;
                }
            }
            if (remove)
                ProductsRemove();
        }
        
        public string FromDB()
        {
            using (ERPDBContext DB = new ERPDBContext())
            {
                try
                { 
                    int industryNumber = DB.Отрасли.Where(i => i.название == industy).Select(i => i.номер).First(); //номер отрасли
                    var productsNumbers = DB.Принадлежности_продуктов.Where(i => i.отрасль == industryNumber).Select(i => i.продукт); //номера продуктов отрасли  
                                        
                    products = new List<Product>(); //заполнение строками с продуктами
                    foreach (var num in productsNumbers)
                        products.Add(new Product(DB.Продукты.Where(i => i.номер == num), ""));
                    if (products.Count() <= 2)
                        remove = false;

                    SizeSelect(DB); //подбор по размеру

                    if (deploy != "0") //подбор по развёртыванию
                        DeploySelect(DB);

                    if (RFLaws != 0) //подбор по учёту закона
                        RFSelect(DB);

                    if (modules.Count != 0) //подбор по модулям
                        ModulesSelect(DB);

                    if (integration.Count != 0) //подбор по интеграциям
                        IntegrationSelect(DB);

                    if (modular != 0) //подбор по модульности
                        ModularSelect(DB);

                    if (trial != 0) //подбор по наличию пробного периода
                        TrialSelect(DB);

                    if (costMonth != int.MaxValue || costForever != int.MaxValue) //подбор по стоимости
                        CostSelect(DB);

                    if (fSize != size) //подбор по перспективному размеру
                        FSizeSelect(DB);

                    if (fModules.Count != 0) //подбор по перспективным модулям
                        FModulesSelect(DB);

                    return ToOutputString(DB);
                }
                catch (Exception)
                {
                    MessageBox.Show("БД не найдена, либо повреждена!\nОбратитесь к программисту для её восстановления!", "ВНИМАНИЕ!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "";
                }
            }
        }
    }
}
