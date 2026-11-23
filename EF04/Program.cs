using EF04.Data;
using EF04.Enums;
using EF04.Models;
using Microsoft.EntityFrameworkCore;

namespace EF04
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new BankDbContext();

            bool exit = false;

            while (!exit)
            {
                Console.Clear();

                Console.WriteLine("====================================");
                Console.WriteLine("     National Bank - Management");
                Console.WriteLine("====================================");
                Console.WriteLine("1) Add a new Customer");
                Console.WriteLine("2) Open a new Account for a Customer");
                Console.WriteLine("3) Update Account Status");
                Console.WriteLine("4) Remove an Account from a Customer");
                Console.WriteLine("5) List all Customers (with accounts)");
                Console.WriteLine("0) Exit");
                Console.WriteLine("------------------------------------");
                Console.Write("Enter choice: ");

                string choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddCustomer(context);
                            break;

                        case "2":
                            OpenAccount(context);
                            break;

                        case "3":
                            UpdateAccountStatus(context);
                            break;

                        case "4":
                            RemoveAccount(context);
                            break;

                        case "5":
                            ListCustomers(context);
                            break;

                        case "0":
                            exit = true;
                            Console.WriteLine("Goodbye!");
                            break;

                        default:
                            Console.WriteLine("Invalid choice!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Operation failed: " + ex.Message);
                }

                if (!exit)
                {
                    Console.WriteLine(
                        "\nPress any key to return to the menu...");
                    Console.ReadKey();
                }
            }
        }

        //Add Customer
        static void AddCustomer(BankDbContext context)
        {
            Console.Clear();
            Console.WriteLine("--- Add New Customer ---");

            Console.Write("Full Name: ");
            string name = Console.ReadLine();

            Console.Write("National ID: ");
            string nationalId = Console.ReadLine();

            Console.Write("Date of Birth (yyyy-MM-dd): ");
            DateTime dob;

            while (!DateTime.TryParse(
                Console.ReadLine(), out dob))
            {
                Console.Write("Invalid date. Enter again: ");
            }

            Console.Write("Email: ");
            string email = Console.ReadLine();

            Console.Write("Phone: ");
            string phone = Console.ReadLine();

            Console.Write("Address: ");
            string address = Console.ReadLine();

            Console.Write("Nationality: ");
            string nationality = Console.ReadLine();

            Console.WriteLine("Customer Type:");
            Console.WriteLine("1) Individual");
            Console.WriteLine("2) Business");
            Console.Write("Choice: ");

            CustomerType type;

            while (true)
            {
                string input = Console.ReadLine();

                if (input == "1")
                {
                    type = CustomerType.Individual;
                    break;
                }

                if (input == "2")
                {
                    type = CustomerType.Business;
                    break;
                }

                Console.Write("Invalid choice. Enter 1 or 2: ");
            }

            var customer = new Customer
            {
                FullName = name,
                NationalId = nationalId,
                DateOfBirth = dob,
                Email = email,
                PhoneNumber = phone,
                Address = address,
                Nationality = nationality,
                CustomerType = type
            };

            context.Customers.Add(customer);
            context.SaveChanges();

            Console.WriteLine(
                $"\nCustomer created successfully. CustomerId = {customer.Id}");
        }

        //Open Account
        static void OpenAccount(BankDbContext context)
        {
            Console.Clear();
            Console.WriteLine("--- Open New Account ---");

            Console.Write("Account Number: ");
            string accountNumber = Console.ReadLine();

            if (context.Accounts.Any(a =>
                a.AccountNumber == accountNumber))
            {
                Console.WriteLine("Account already exists!");
                return;
            }

            Console.WriteLine("Account Type:");
            Console.WriteLine("1) Savings");
            Console.WriteLine("2) Current");
            Console.WriteLine("3) Business");
            Console.Write("Choice: ");

            AccountType accountType;

            string typeChoice = Console.ReadLine();

            if (typeChoice == "1")
                accountType = AccountType.Savings;
            else if (typeChoice == "2")
                accountType = AccountType.Current;
            else if (typeChoice == "3")
                accountType = AccountType.Business;
            else
            {
                Console.WriteLine("Invalid account type!");
                return;
            }

            Console.Write("Branch Code: ");
            string branchCode = Console.ReadLine();

            Console.Write("Customer Id: ");
            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid Customer Id!");
                return;
            }

            Console.WriteLine("Ownership Role:");
            Console.WriteLine("1) Primary");
            Console.WriteLine("2) CoHolder");
            Console.Write("Choice: ");

            OwnershipType ownershipType;

            string ownershipChoice = Console.ReadLine();

            if (ownershipChoice == "1")
                ownershipType = OwnershipType.Primary;
            else if (ownershipChoice == "2")
                ownershipType = OwnershipType.CoHolder;
            else
            {
                Console.WriteLine("Invalid ownership role!");
                return;
            }

            // Validate Branch
            var branch = context.Branches
                .FirstOrDefault(b => b.BranchCode == branchCode);

            if (branch == null)
            {
                Console.WriteLine("Branch not found!");
                return;
            }

            // Validate Customer
            var customer = context.Customers
                .FirstOrDefault(c => c.Id == customerId);

            if (customer == null)
            {
                Console.WriteLine("Customer not found!");
                return;
            }

            // Create Account
            var account = new Account
            {
                AccountNumber = accountNumber,
                AccountType = accountType,
                CurrentBalance = 0,
                OpeningDate = DateTime.Now,
                BranchCode = branchCode
            };

            // Create CustomerAccount
            var customerAccount = new CustomerAccount
            {
                AccountNumber = accountNumber,
                CustomerId = customerId,
                OwnershipType = ownershipType,
                OwnershipStartDate = DateTime.Now,
                AccountStatus = AccountStatus.Active
            };

            context.Accounts.Add(account);
            context.CustomerAccounts.Add(customerAccount);

            context.SaveChanges();

            Console.WriteLine(
                $"Account '{accountNumber}' created and linked to customer {customerId} as {ownershipType} owner.");
        }

        //Update Account Status
        static void UpdateAccountStatus(BankDbContext context)
        {
            Console.Clear();
            Console.WriteLine("--- Update Account Status ---");

            Console.Write("Account Number: ");
            string accountNumber = Console.ReadLine();

            Console.Write("Customer Id: ");

            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid Customer Id!");
                return;
            }

            var customerAccount = context.CustomerAccounts
                .FirstOrDefault(ca =>
                    ca.AccountNumber == accountNumber &&
                    ca.CustomerId == customerId);

            if (customerAccount == null)
            {
                Console.WriteLine("Account ownership not found!");
                return;
            }

            Console.WriteLine("New Status:");
            Console.WriteLine("1) Active");
            Console.WriteLine("2) Closed");
            Console.Write("Choice: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                customerAccount.AccountStatus = AccountStatus.Active;
            }
            else if (choice == "2")
            {
                customerAccount.AccountStatus = AccountStatus.Closed;
            }
            else
            {
                Console.WriteLine("Invalid choice!");
                return;
            }

            context.SaveChanges();

            Console.WriteLine(
                $"Status updated to {customerAccount.AccountStatus}.");
        }

        //Remove Account
        static void RemoveAccount(BankDbContext context)
        {
            Console.Clear();
            Console.WriteLine("--- Remove Account From Customer ---");

            Console.Write("Account Number: ");
            string accountNumber = Console.ReadLine();

            Console.Write("Customer Id: ");

            if (!int.TryParse(Console.ReadLine(), out int customerId))
            {
                Console.WriteLine("Invalid Customer Id!");
                return;
            }

            var customerAccount = context.CustomerAccounts
                .FirstOrDefault(ca =>
                    ca.AccountNumber == accountNumber &&
                    ca.CustomerId == customerId);

            if (customerAccount == null)
            {
                Console.WriteLine("Ownership link not found!");
                return;
            }

            // Delete CustomerAccount
            context.CustomerAccounts.Remove(customerAccount);

            // Check if other customers own this account
            bool hasOtherOwners = context.CustomerAccounts.Any(
                ca => ca.AccountNumber == accountNumber &&
                      ca.CustomerId != customerId);

            if (!hasOtherOwners)
            {
                var account = context.Accounts
                    .FirstOrDefault(a => a.AccountNumber == accountNumber);

                if (account != null)
                {
                    context.Accounts.Remove(account);
                }

                Console.WriteLine("Ownership link deleted.");
                Console.WriteLine(
                    $"This was the last owner - account '{accountNumber}' was also removed.");
            }
            else
            {
                Console.WriteLine("Ownership link deleted.");
            }

            context.SaveChanges();
        }

        //List Customers
        static void ListCustomers(BankDbContext context)
        {
            Console.Clear();
            Console.WriteLine("--- All Customers ---");

            var customers = context.Customers
                .Include(c => c.CustomerAccounts)
                    .ThenInclude(ca => ca.Account)
                        .ThenInclude(a => a.Branch)
                .ToList();

            foreach (var customer in customers)
            {
                Console.WriteLine(
                    $"\n#{customer.Id} {customer.FullName} ({customer.CustomerType})");

                if (!customer.CustomerAccounts.Any())
                {
                    Console.WriteLine("   (no accounts)");
                    continue;
                }

                foreach (var ca in customer.CustomerAccounts)
                {
                    Console.WriteLine(
                        $"   {ca.Account.AccountNumber} - " +
                        $"{ca.Account.AccountType} " +
                        $"Balance: {ca.Account.CurrentBalance:N2} " +
                        $"{ca.OwnershipType} " +
                        $"{ca.AccountStatus} " +
                        $"@ {ca.Account.Branch.Name}");
                }
            }
        }


    }

}
