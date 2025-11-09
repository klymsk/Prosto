using Prosto.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prosto.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db, int itemsCount = 15000)
        {
            await db.Database.EnsureCreatedAsync();

            // 1️⃣ Продавці
            if (!await db.Sellers.AnyAsync())
            {
                Console.WriteLine("Додаю продавців...");
                var sellers = new List<Seller>();
                for (int i = 1; i <= 10; i++)
                    sellers.Add(new Seller { SellerName = $"Продавець #{i}" });
                db.Sellers.AddRange(sellers);
                await db.SaveChangesAsync();
            }

            // створюємо один спільний генератор випадкових чисел 👇
            var rnd = new Random();

            // 2️⃣ Товари
            if (!await db.Items.AnyAsync())
            {
                Console.WriteLine($"Додаю {itemsCount} товарів...");
                var sellers = await db.Sellers.ToListAsync();

                var categories = new[] { "Смартфони", "Ноутбуки", "Навушники", "Годинники", "Камери" };
                var itemNames = new[] { "Galaxy", "iPhone", "ThinkPad", "MacBook", "Canon", "Xiaomi", "Asus", "Dell", "Sony", "Huawei" };

                var batch = new List<Item>();

                for (int i = 1; i <= itemsCount; i++)
                {
                    var name = $"{itemNames[rnd.Next(itemNames.Length)]} {rnd.Next(100, 999)}";
                    var category = categories[rnd.Next(categories.Length)];
                    var price = Math.Round(rnd.NextDouble() * 5000 + 500, 2);

                    batch.Add(new Item
                    {
                        Name = name,
                        Category = category,
                        Description = $"Опис товару {name} з категорії {category}",
                        Price = (decimal)price,
                        SellerId = sellers[rnd.Next(sellers.Count)].SellerId
                    });

                    // зберігаємо батчами, щоб не навантажувати EF
                    if (batch.Count == 1000)
                    {
                        db.Items.AddRange(batch);
                        await db.SaveChangesAsync();
                        Console.WriteLine($"Додано {i} товарів...");
                        batch.Clear();
                    }
                }

                if (batch.Count > 0)
                {
                    db.Items.AddRange(batch);
                    await db.SaveChangesAsync();
                }

                Console.WriteLine("✅ Завантаження товарів завершено!");
            }

            // 3️⃣ Користувачі
            if (!await db.Customers.AnyAsync())
            {
                Console.WriteLine("Додаю користувачів...");
                var users = new List<Customer>();
                for (int i = 1; i <= 100; i++)
                {
                    users.Add(new Customer
                    {
                        FullName = $"Користувач {i}",
                        Email = $"user{i}@mail.com",
                        Password = "123456",
                        PhoneNumber = $"+38099{rnd.Next(1000000, 9999999)}",
                        AuthProvider = "Local"
                    });
                }
                db.Customers.AddRange(users);
                await db.SaveChangesAsync();
            }

            Console.WriteLine("✅ Базу даних заповнено!");
        }
    }
}