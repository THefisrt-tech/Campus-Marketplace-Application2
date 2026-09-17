using System;
using System.Collections.Generic;
using System.Linq;

namespace CampusMarketplace
{
    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public User(string username, string password)
        {
            Username = username;
            PasswordHash = HashPassword(password);
        }

        private string HashPassword(string password)
        {
            // Simple hash representation for demonstration
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public bool VerifyPassword(string password)
        {
            return PasswordHash == HashPassword(password);
        }
    }

    public class MarketplaceItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string SellerUsername { get; set; }

        public MarketplaceItem(int id, string title, string category, decimal price, string seller)
        {
            Id = id;
            Title = title;
            Category = category;
            Price = price;
            SellerUsername = seller;
        }
    }

    public class MarketplaceService
    {
        private List<User> users = new List<User>();
        private List<MarketplaceItem> items = new List<MarketplaceItem>();
        private int nextItemId = 1;

        public bool RegisterUser(string username, string password)
        {
            if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Registration failed: Username '{username}' is already taken.");
                return false;
            }

            users.Add(new User(username, password));
            Console.WriteLine($"User '{username}' registered successfully!");
            return true;
        }

        public User AuthenticateUser(string username, string password)
        {
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user != null && user.VerifyPassword(password))
            {
                Console.WriteLine($"User '{username}' logged in successfully.");
                return user;
            }

            Console.WriteLine("Authentication failed: Invalid username or password.");
            return null;
        }

        public void PostItem(string title, string category, decimal price, string sellerUsername)
        {
            var item = new MarketplaceItem(nextItemId++, title, category, price, sellerUsername);
            items.Add(item);
            Console.WriteLine($"Listing posted: '{title}' (${price}) by @{sellerUsername}");
        }

        public void SearchItems(string keyword)
        {
            var results = items.Where(i => 
                i.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
                i.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            Console.WriteLine($"\n--- Search Results for '{keyword}' ({results.Count} items found) ---");
            if (!results.Any())
            {
                Console.WriteLine("No listings matched your query.");
                return;
            }

            foreach (var item in results)
            {
                Console.WriteLine($"[ID: {item.Id}] {item.Title} | Category: {item.Category} | Price: ${item.Price:F2} | Seller: @{item.SellerUsername}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            MarketplaceService marketplace = new MarketplaceService();

            // 1. User Registration & Auth
            Console.WriteLine("=== Campus Marketplace Setup ===");
            marketplace.RegisterUser("abel_wolde", "SecurePass123!");
            marketplace.RegisterUser("jane_smith", "CampusPass456!");

            var loggedInUser = marketplace.AuthenticateUser("abel_wolde", "SecurePass123!");

            // 2. Add Listings
            Console.WriteLine("\n=== Posting Listings ===");
            if (loggedInUser != null)
            {
                marketplace.PostItem("Calculus Textbook", "Books", 45.00m, loggedInUser.Username);
                marketplace.PostItem("Wireless Ergonomic Mouse", "Electronics", 20.00m, loggedInUser.Username);
            }
            marketplace.PostItem("CS Lab Notebook", "Books", 12.50m, "jane_smith");

            // 3. Search Listings
            Console.WriteLine("\n=== Searching Marketplace ===");
            marketplace.SearchItems("Books");
            marketplace.SearchItems("Mouse");
        }
    }
}
