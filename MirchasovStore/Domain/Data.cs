﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MirchasovStore.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MirchasovStore;

public static class Data
{
    public static IMongoDatabase DB;

    public static List<Product> ExistingTovars;

    public static List<string> Razdels;
    public static List<string> Categories;
    public static List<List<string>> Tree;

    public static List<string> Levels;

    public static IMongoCollection<Product> productsCollection;


    public static void InitData(IConfiguration Configuration)
    {
        var dbConfigSection = Configuration.GetSection("DBConfig");
        var dbConfig = new DBConf(dbConfigSection);
        var mongoClient = new MongoClient(dbConfig.ConnectionString);
        DB = mongoClient.GetDatabase(dbConfig.DBName);

        //Каталог и все такое
        productsCollection = DB.GetCollection<Product>("products");
        ExistingTovars = GetAllProducts();
        Categories = ExistingTovars.Select(x => x.CatLev[2]).Distinct().OrderBy(x => x).ToList();       //Категории через уровни иерархии
        //Categories = ExistingTovars.Select(x => x.CatLev[1]).Distinct().OrderBy(x => x).ToList();       //Разделы через уровни иерархии
        //Razdels = ExistingTovars.Select(x => x.CatLev[1]).Distinct().OrderBy(x => x).ToList();       //Разделы через уровни иерархии
        //Categories = ExistingTovars.Select(x => x.BrandName).Distinct().OrderBy(x => x).ToList();         //Категории через Бренды 


        var group1 = ExistingTovars.GroupBy(p => p.CatLev[2]).ToList();
        var group2 = ExistingTovars.GroupBy(p => p.BrandName);




    }

    private static List<Product> GetAllProducts()
    {
        BsonDocument filter = new BsonDocument();
        return productsCollection.Find(filter).ToList();
    }

    public static double TryParseDouble(string src, double Default)
    {
        if (string.IsNullOrEmpty(src))
            return Default;

        if (double.TryParse(src.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double Result))
            return Result;

        return Default;
    }

    public static bool Set<TItem, TValue>(this TItem item, Expression<Func<TItem, TValue>> expression, TValue newValue) where TItem : Product
    {
        Type TDest = item.GetType();

        if (expression == null) throw new ArgumentNullException(nameof(expression));
        if (expression.Body is not MemberExpression memberExp)
            throw new Exception("Only member access expressions may be used");

        string memberName = memberExp.Member.Name;
        var prop = TDest.GetProperty(memberName, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
        //if (PropertiesToSkip.Contains(prop.Name)) return false; // !!!
        if (!prop.CanWrite) return false;

        object value = prop.GetValue(item);
        if (Equals(value, newValue)) return false;

        prop.SetValue(item, newValue);
        return true;
    }

    public static bool AllItemsEqual<T>(this List<T> L1, List<T> L2) where T : class
    {
        if (L1 == L2) return true;
        if (L1 == null || L2 == null) return false;
        if (L1.Count != L2.Count) return false;
        for (int i = 0; i < L1.Count; i++)
        {
            if (!Equals(L1[i], L2[i])) return false;
        }
        return true;
    }


    public static void ImportCSV()
    {
        string[] lines = System.IO.File.ReadAllLines(@"D:\tovar.csv", System.Text.Encoding.GetEncoding(1251));
        CSVFile csv = new(lines, ';', '"',
            "Код;Иерархия;Наименование;Артикул;Количество;Цена;Бренд;ИмяФайлаИзображения;Новинка;HAPPY_МОЛЛОстаток;ГлобусОстаток;ЛазурныйОстаток;ОранжевыйОстаток;ПассажОстаток;ТауОстаток;10 летняя батарея;Bluetooth;Барометр\\альтиметр;Браслет;Будильник;Включение,отключение звука кнопок;Водозащита;Грязеустойчивость;Дополнительно;Индикатор приливов и отливов;Индикатор уровня заряда аккумулятора;Компас;Материал корпуса;Мелодия;Мировое время;Отображение данных о Луне;Пол;Прием радиосигнала точного времени;Размер корпуса;Резерв хода;Сверх яркая подсветка;Связь со смартфоном;Скидка;Солнечная батарея;Стекло;Страна бренда;Страна производитель;Таймер;Таймер рыбалки;Термометр;Тип механизма;Ударопрочность;Устойчивость к воздействию магнитного поля;Форма корпуса;Функция поиска телефона;Функция секундомера;Хит продаж;Ход стрелки;Хронограф;Циферблат;Шагомер;ЦенаСоСкидкой",
            "Code1C;Path;Name;Article;TotalCount;Price;BrandName;ImgFileName;FlagNew;HM_Balance;GL_Balance;LZ_Balance;OR_Balance;PZ_Balance;TA_Balance;Battery10;Bluetooth;Barometer;Wristlet;Alarm;ButtonsSoundToggler;WaterProtection;DirtResistance;Extra;TideIndicator;BatteryLevelIndicator;Compass;CaseMaterial;Melody;WorldTime;MoonData;Gender;ExactTimeRadioSignal;CaseSize;PowerReserve;ExtraBrightBacklight;SmartphoneConnection;Discount;SolarBattery;Glass;BrandCountry;ProducingCountry;Taimer;FishingTimer;Thermometer;MechanismType;ImpactResistance;MagneticFieldResistance;CaseForm;PhoneSearchFunction;Stopwatch;FlagSaleLeader;ClockhandMovement;Chronograph;ClockFace;Pedometer;DiscountPrice");
        csv.Rewind();

        List<Product> Tovars = new();

        while (csv.Next())
        {
            // double.TryParse(csv["Discount"], out double Discount); - создана переменная, действие с ней, а потом присвоение внутри new Product значения переменной к полю (Discount = Discount)
            // сейчас универсальная функция TryParseDouble() и ее вызов с обязательным параметром по умолчанию

            string CategoriesString = csv["Path"];
            var CatLev = CategoriesString.Split('/').ToList();
            string code1c = csv["Code1C"];
            //Console.WriteLine(Categories[1]);

            var tovar = ExistingTovars.FirstOrDefault(x => x.Code1C == code1c);
            if (tovar == null) tovar = new() { Id = ObjectId.GenerateNewId(), Code1C = code1c };

            bool mod = false;
            mod |= tovar.Set(x => x.Name, csv["Name"]);
            mod |= tovar.Set(x => x.Path, csv["Path"]);
            if (!tovar.CatLev.AllItemsEqual(CatLev))
            {
                tovar.CatLev = CatLev;
                mod = true;
            }
            mod |= tovar.Set(x => x.Article, csv["Article"]);
            mod |= tovar.Set(x => x.BrandName, csv["BrandName"]);
            mod |= tovar.Set(x => x.ImgFileName, csv["ImgFileName"]);
            mod |= tovar.Set(x => x.Price, double.Parse(csv["Price"]));
            mod |= tovar.Set(x => x.Discount, TryParseDouble(csv["Discount"], 0.0));
            mod |= tovar.Set(x => x.DiscountPrice, TryParseDouble(csv["DiscountPrice"], 0.0));
            mod |= tovar.Set(x => x.FlagNew, csv["FlagNew"] == "1");
            mod |= tovar.Set(x => x.FlagSaleLeader, csv["FlagSaleLeader"] == "Да");
            mod |= tovar.Set(x => x.TotalCount, Int32.Parse(csv["TotalCount"]));
            mod |= tovar.Set(x => x.HM_Balance, Int32.Parse(csv["HM_Balance"]));
            mod |= tovar.Set(x => x.GL_Balance, Int32.Parse(csv["GL_Balance"]));
            mod |= tovar.Set(x => x.LZ_Balance, Int32.Parse(csv["LZ_Balance"]));
            mod |= tovar.Set(x => x.OR_Balance, Int32.Parse(csv["OR_Balance"]));
            mod |= tovar.Set(x => x.PZ_Balance, Int32.Parse(csv["PZ_Balance"]));
            mod |= tovar.Set(x => x.TA_Balance, Int32.Parse(csv["TA_Balance"]));
            mod |= tovar.Set(x => x.Battery10, csv["Battery10"]);
            mod |= tovar.Set(x => x.Bluetooth, csv["Bluetooth"]);
            mod |= tovar.Set(x => x.Barometer, csv["Barometer"]);
            mod |= tovar.Set(x => x.Wristlet, csv["Wristlet"]);
            mod |= tovar.Set(x => x.Alarm, csv["Alarm"]);
            mod |= tovar.Set(x => x.ButtonsSoundToggler, csv["ButtonsSoundToggler"]);
            mod |= tovar.Set(x => x.WaterProtection, csv["WaterProtection"]);
            mod |= tovar.Set(x => x.DirtResistance, csv["DirtResistance"]);
            mod |= tovar.Set(x => x.Extra, csv["Extra"]);
            mod |= tovar.Set(x => x.TideIndicator, csv["TideIndicator"]);
            mod |= tovar.Set(x => x.BatteryLevelIndicator, csv["BatteryLevelIndicator"]);
            mod |= tovar.Set(x => x.Compass, csv["Compass"]);
            mod |= tovar.Set(x => x.CaseMaterial, csv["CaseMaterial"]);
            mod |= tovar.Set(x => x.Melody, csv["Melody"]);
            mod |= tovar.Set(x => x.WorldTime, csv["WorldTime"]);
            mod |= tovar.Set(x => x.MoonData, csv["MoonData"]);
            mod |= tovar.Set(x => x.Gender, csv["Gender"]);
            mod |= tovar.Set(x => x.ExactTimeRadioSignal, csv["ExactTimeRadioSignal"]);
            mod |= tovar.Set(x => x.CaseSize, csv["CaseSize"]);
            mod |= tovar.Set(x => x.PowerReserve, csv["PowerReserve"]);
            mod |= tovar.Set(x => x.ExtraBrightBacklight, csv["ExtraBrightBacklight"]);
            mod |= tovar.Set(x => x.SmartphoneConnection, csv["SmartphoneConnection"]);
            mod |= tovar.Set(x => x.SolarBattery, csv["SolarBattery"]);
            mod |= tovar.Set(x => x.Glass, csv["Glass"]);
            mod |= tovar.Set(x => x.BrandCountry, csv["BrandCountry"]);
            mod |= tovar.Set(x => x.ProducingCountry, csv["ProducingCountry"]);
            mod |= tovar.Set(x => x.Taimer, csv["Taimer"]);
            mod |= tovar.Set(x => x.FishingTimer, csv["FishingTimer"]);
            mod |= tovar.Set(x => x.Thermometer, csv["Thermometer"]);
            mod |= tovar.Set(x => x.MechanismType, csv["MechanismType"]);
            mod |= tovar.Set(x => x.ImpactResistance, csv["ImpactResistance"]);
            mod |= tovar.Set(x => x.MagneticFieldResistance, csv["MagneticFieldResistance"]);
            mod |= tovar.Set(x => x.CaseForm, csv["CaseForm"]);
            mod |= tovar.Set(x => x.PhoneSearchFunction, csv["PhoneSearchFunction"]);
            mod |= tovar.Set(x => x.Stopwatch, csv["Stopwatch"]);
            mod |= tovar.Set(x => x.ClockhandMovement, csv["ClockhandMovement"]);
            mod |= tovar.Set(x => x.Chronograph, csv["Chronograph"]);
            mod |= tovar.Set(x => x.ClockFace, csv["ClockFace"]);
            mod |= tovar.Set(x => x.Pedometer, csv["Pedometer"]);

            if (mod)
            {
                productsCollection.ReplaceOne(new BsonDocument() { { "_id", tovar.Id } }, tovar, AlwaysUpsert);
            }

            //Console.WriteLine($"Код1C: {csv["Code1C"]}, Наименование: {csv["Name"]}, Артикул: {csv["Article"]}, Цена: {csv["Price"]}");

        }

    }

    static ReplaceOptions AlwaysUpsert = new() { IsUpsert = true };


}
