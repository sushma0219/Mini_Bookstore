using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _671Books.Models;
using System.Text;

namespace _671Books.Controllers
{
    public class HomeController : Controller
    {
       

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "671 Books";

            return View();
        }


        public ActionResult Index()
        {
            ViewBag.Message = "";
            BookStoreEntities book = new BookStoreEntities();                    


            List<_671Books.Models.Book> _books = new List<_671Books.Models.Book>();
            using (BookStoreEntities bookEntities = new BookStoreEntities())
            {
                //_books = bookEntities.Books.ToList();
                var books = bookEntities.books.Where(b => b.qtyinstore > 0).ToList();
                foreach (var b in books)
                {

                    _671Books.Models.Book _book = new _671Books.Models.Book();
                    _book.ID = b.bid;
                    _book.Name = b.title;
                    _book.Author = b.author;
                    _book.Genre = b.genre;
                    _book.Year = b.year;
                    _book.Edition = b.edition;
                    _book.Publisher = b.publisher;
                    _book.Price = b.price;
                    _book.QuantityInStore = b.qtyinstore;
                    //_book.QuantitySold = b.qtysold;
                    //_book.BookCover = b.BookCover;

                    _books.Add(_book);

                }
                return View(_books);

            }

        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {

            //List<Book> _books = new List<Book>();
            List<_671Books.Models.Book> _books = new List<_671Books.Models.Book>();
            using (BookStoreEntities bookEntities = new BookStoreEntities())
            {
                
                var Author = formCollection["Author"];
                var Genre = (formCollection["Genres"]);
                var Year = (formCollection["Year"]);

                var books = bookEntities.books.ToList();

                if (Author.Length != 0)
                {
                    books = books.Where(b => b.author.ToLower().Contains(Author.ToLower()) && b.qtyinstore > 0).ToList();
                }
                if (Genre.Length != 0)
                {
                    books = books.Where(b => b.genre.ToLower().Contains(Genre.ToLower()) && b.qtyinstore > 0).ToList();
                }
                if (Year.Length != 0)
                {
                    books = books.Where(b => b.year.ToString() == Year && b.qtyinstore > 0).ToList();
                }
                foreach (var book in books)
                {
                    _671Books.Models.Book _book = new _671Books.Models.Book();
                    _book.ID = book.bid;
                    _book.Name = book.title;
                    _book.Author = book.author;
                    _book.Genre = book.genre;
                    _book.Year = book.year;
                    _book.Edition = book.edition;
                    _book.Publisher = book.publisher;
                    _book.Price = book.price;
                    _book.QuantityInStore = book.qtyinstore;
                    //_book.QuantitySold = book.qtysold;
                    _books.Add(_book);
                }
                return View(_books);
            }

        }

        public ActionResult BookDetails(string ID, string Source)
        {
            _671Books.Models.Book _book = new _671Books.Models.Book();
            BookStoreEntities book = new BookStoreEntities();
            var books = book.books.ToList();
            books = books.Where(b => b.bid.ToString() == ID).ToList();
            if (Source != "Index")
            {
                ViewBag.DisplayButton = "display:none";
                ViewBag.DisplayRemoveButton = "display:block";
            }
            else {
                ViewBag.DisplayButton = "display:block";
                ViewBag.DisplayRemoveButton = "display:none";
            }
            foreach (var b in books)
            {
                _book.ID = b.bid;
                _book.Name = b.title;
                _book.Author = b.author;
                _book.Genre = b.genre;
                _book.Year = b.year;
                _book.Edition = b.edition;
                _book.Publisher = b.publisher;
                _book.Price = b.price;
                _book.QuantityInStore = b.qtyinstore;
                //_book.QuantitySold = b.qtysold;
               // _book.BookCover = b.BookCover;
            }
            return View(_book);
        }

        [HttpPost]
        public ActionResult BookDetails(FormCollection formCollection, string wishlist, string addtocart, string continuetoshopping, string removewishlist)
        {
            var ID = formCollection["txtID"];
            var QtyInStore = formCollection["txtQtyInStore"];
            string userSettings="0";
            if (Request.Cookies["UserSettings"] != null)
            {
                
                if (Request.Cookies["UserSettings"]["UserID"] != null)
                { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
            }
            
            if (wishlist != null)
            {
                using (BookStoreEntities book = new BookStoreEntities())
                {
                    var alreadyexisting = book.Carts.Where(b => b.bid.ToString() == ID && b.cid.ToString() == userSettings).ToList();
                    if(alreadyexisting.Count() == 0)
                    {
                        var _bookCart = book.books.Where(b => b.bid.ToString() == ID).ToList();
                        foreach (var b in _bookCart)
                        {
                            //Add customerid as well
                            book.Carts.Add(new Cart { cid = Convert.ToInt32(userSettings), bid = b.bid, title = b.title, author = b.author, edition = b.edition, genre = b.genre, price = b.price, publisher = b.publisher, inWishList = true, orderStatus = "WishList", year = b.year, qtyinstore = b.qtyinstore });
                            book.SaveChanges();
                        }
                    }
                    

                }
                return RedirectToAction("Index", "Home");
            }
            if (addtocart != null)
            {
                
                using (BookStoreEntities book = new BookStoreEntities())
                {
                    var alreadyexisting = book.Carts.SingleOrDefault(b => b.bid.ToString() == ID && b.cid.ToString() == userSettings);
                    if (alreadyexisting == null)
                    {
                        var _bookCart = book.books.Where(b => b.bid.ToString() == ID).ToList();
                        foreach (var b in _bookCart)
                        {
                            //Add customerid as well
                            book.Carts.Add(new Cart { cid = Convert.ToInt32(userSettings), bid = b.bid, title = b.title, author = b.author, edition = b.edition, genre = b.genre, price = b.price, publisher = b.publisher, inWishList = false, orderStatus = "Placed", year = b.year, qtyinstore = b.qtyinstore });
                            book.SaveChanges();
                        }
                    }
                    else
                    {
                        if (alreadyexisting.inWishList == true)
                        {
                            var cid = Convert.ToInt32(userSettings);
                            var bid = Convert.ToInt32(ID);
                            var _bookCart = book.books.Where(b => b.bid.ToString() == ID).ToList();

                            var bookToRemove = book.Carts.Where(b => b.bid.ToString() == ID && b.cid.ToString() == userSettings);
                            book.Carts.RemoveRange(bookToRemove);
                            book.SaveChanges();

                            foreach (var b in _bookCart)
                            {                                
                                book.Carts.Add(new Cart { cid = Convert.ToInt32(userSettings), bid = b.bid, title = b.title, author = b.author, edition = b.edition, genre = b.genre, price = b.price, publisher = b.publisher, inWishList = false, orderStatus = "Placed", year = b.year, qtyinstore = b.qtyinstore });
                                book.SaveChanges();
                            }
                        }
                    }
                   
                    
                }
                return RedirectToAction("Index", "Home");
            }
            if(removewishlist != null)
            {
                using (BookStoreEntities book = new BookStoreEntities())
                {
                    var bookToRemove = book.Carts.Where(b => b.bid.ToString() == ID && b.cid.ToString() == userSettings);
                    book.Carts.RemoveRange(bookToRemove);
                    book.SaveChanges();
                }
                return RedirectToAction("WishList", "Home");
            }
            if (continuetoshopping != null)
            {
            }
           return RedirectToAction("Index", "Home");
        }

     


        public ActionResult WishList()
        {
            List<_671Books.Models.Book> _books = new List<_671Books.Models.Book>();
            
            string userSettings = "";
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"]["UserID"] != null)
                { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
            }
            using (BookStoreEntities _book = new BookStoreEntities())
            {
                var userBooks = _book.Carts.Where(b => b.cid.ToString() == userSettings && b.inWishList == true).ToList();
                foreach (var b in userBooks)
                {
                    _671Books.Models.Book book = new _671Books.Models.Book();
                    book.Author = b.author;
                    //book.BookCover
                    book.Edition = b.edition;
                    book.Genre = b.genre;
                    book.ID = b.bid;
                    //book.ISBN = b.i
                    book.Name = b.title;
                    book.Price = b.price;
                    book.Publisher = b.publisher;
                    
                    book.Year = b.year;
                    _books.Add(book);
                }
            }
            return View(_books);
        }

        public ActionResult RemoveCart(string ID)
        {
            string userSettings = "0";
            if (Request.Cookies["UserSettings"] != null)
            {
                
                if (Request.Cookies["UserSettings"]["UserID"] != null)
                { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
            }
            using (BookStoreEntities book = new BookStoreEntities())
            {
                var bookToRemove = book.Carts.Where(b => b.bid.ToString() == ID && b.cid.ToString() == userSettings);
                book.Carts.RemoveRange(bookToRemove);
                book.SaveChanges();
            }
            return RedirectToAction("Cart", "Home");
        }

        public ActionResult Cart()
        {
            List<_671Books.Models.Book> _books = new List<_671Books.Models.Book>();
            
            string userSettings = "0";
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"]["UserID"] != null)
                { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
            }
            using (BookStoreEntities _book = new BookStoreEntities())
            {
                var userBooks = _book.Carts.Where(b => b.cid.ToString() == userSettings && b.inWishList == false && b.orderStatus == "Placed").ToList();
                foreach (var b in userBooks)
                {
                    _671Books.Models.Book book = new _671Books.Models.Book();
                    book.Author = b.author;
                    //book.BookCover
                    book.Edition = b.edition;
                    book.Genre = b.genre;
                    book.ID = b.bid;
                    //book.ISBN = b.i
                    book.Name = b.title;
                    book.Price = b.price;
                    book.Publisher = b.publisher;
                    book.QuantityInStore = b.qtyinstore;
                    book.Year = b.year;
                    _books.Add(book);
                }
                //ViewBag.ModelCount = _books.Count();
                //for (var i = 0; i < userBooks.Count; i++ )
                //{
                //    book.Author = userBooks[i].author;
                //    //book.BookCover
                //    book.Edition = userBooks[i].edition;
                //    book.Genre = userBooks[i].genre;
                //    book.ID = userBooks[i].bid;
                //    //book.ISBN = b.i
                //    book.Name = userBooks[i].title;
                //    book.Price = userBooks[i].price;
                //    book.Publisher = userBooks[i].publisher;
                //    book.QuantityInStore = userBooks[i].qtyinstore;
                //    book.Year = userBooks[i].year;
                //    _books.Add(book);
                //}
            }
            return View(_books);
        }

        [HttpPost]
        public ActionResult Cart(FormCollection formCollection)
        {
            int n = 0;
            var Name = formCollection["Name"];
            var ShippingAddress = formCollection["ShippingAddress"];
            var BillingAddress = formCollection["BillingAddress"];
            var Card = formCollection["Card"];
            var CVV = formCollection["CVV"];
            long cardInt;
            bool isCardNumerical =  Int64.TryParse(Card, out cardInt);
            //string error = "";
            int cvvInt;
            bool isCVVNumerical = Int32.TryParse(CVV, out cvvInt);
            var error = new StringBuilder();
            
            if (string.IsNullOrEmpty(Name))
            {
                error.AppendLine("Please enter a Name");
                error.AppendLine();
                //error = error + "Please enter a Name" + "\n" ;
            }
            if (string.IsNullOrEmpty(ShippingAddress))
            {
                //error = error  + "Please enter Shipping Address" + "\n";
                error.AppendLine("Please enter Shipping Address");
                error.AppendLine();
            }
            if (string.IsNullOrEmpty(BillingAddress))
            {
                //error = error + "Please enter Billing Address" + "\n";
                error.Append("Please enter Billing Address");
                error.AppendLine();
            }
            if (string.IsNullOrEmpty(Card))
            {
                //error = error + "Please enter Credit Card Number" + "\n";
                error.Append("Please enter Credit Card Number");
                error.AppendLine();
            }
            if (string.IsNullOrEmpty(CVV))
            {
                //error = error + "Please enter CVV Number" + "\n";
                error.Append("Please enter CVV Number");
                error.AppendLine();
            }
            if (!isCardNumerical)
            {
                //error = error + "Please enter valid Credit Card Number" + "\n";
                error.Append("Please enter valid Credit Card Number");
                error.AppendLine();
            }
            if (!isCVVNumerical)
            {
                //error = error + "Please enter valid CVV Number" + "\n";
                error.Append("Please enter valid CVV Number");
                error.AppendLine();
            }
            error = error.Replace("\n", Environment.NewLine);
            ViewBag.SummaryDiv = error.ToString();
            List<_671Books.Models.Book> _books = new List<_671Books.Models.Book>();
            if (error.Length == 0)
            {
                List<string> ID = new List<string>();
                List<string> a = new List<string>();
                int count = 0;
                string value = "";
                decimal total = 0;
                //string[] a = new string[100];
                string userSettings = "0";
                if (Request.Cookies["UserSettings"] != null)
                {
                    if (Request.Cookies["UserSettings"]["UserID"] != null)
                    { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
                }
                using (BookStoreEntities book = new BookStoreEntities())
                {
                    foreach (var key in formCollection.AllKeys)
                    {
                        if (key != "Name" && key != "ShippingAddress" && key != "BillingAddress" && key != "Card" && key != "MM" && key != "YY" && key != "CVV" && key != "placeOrder")
                        {
                            if (key.Contains("txt"))
                            {
                                value = formCollection["txt"];
                                //a = value.Split(',');

                            }
                            else
                            {
                                var v = key;
                                ID.Add(v);
                            }
                        }

                        

                        //var value = formCollection[key];
                        //var ID = key.Substring(2, key.Length - 3);
                        //if (key.Contains("txt"))
                        //{
                        //    var value = formCollection[key];
                        //    var ID = key.Substring(3, key.Length - 3);
                        //
                    }
                    Random r = new Random();
                    n = r.Next(1000001, 9999999);

                    string[] b = value.Split(',');
                    for (int j = 0; j < b.Count(); j++)
                    {
                        a.Add(b[j]);
                    }
                    var bookToRemove = book.Carts.Where(_book => _book.cid.ToString() == userSettings && _book.orderStatus == "Placed");
                    book.Carts.RemoveRange(bookToRemove);
                    book.SaveChanges();

                    decimal qty = 0;
                    for (int i = 0; i < a.Count(); i++)
                    {
                        var id = ID[i];
                        var originalRecord = book.books.FirstOrDefault(x => x.bid.ToString() == id);
                        if (originalRecord != null)
                        {
                            var num = Convert.ToDecimal(a[i]);
                            total = total + num;
                            var den = Convert.ToDecimal(originalRecord.price);
                            qty = num / den;
                            //book.Carts.Add(new Cart { cid = Convert.ToInt32(userSettings), bid = originalRecord.bid, price = Convert.ToDecimal(a[i]), title = originalRecord.title, qty =  (int)qty, edition = originalRecord.edition, year = originalRecord.year, author = originalRecord.author, publisher = originalRecord.publisher, genre = originalRecord.genre,inWishList = false, orderStatus = "Ordered", qtyinstore = originalRecord.qtyinstore});
                            //book.SaveChanges();
                            book.Orders.Add(new Order { cid = Convert.ToInt32(userSettings), bid=originalRecord.bid, orderdate = DateTime.Now, shippingaddress = ShippingAddress, Status = "Ordered", totalprice = num, orderid = n, qtysold = Convert.ToInt32(qty) });
                            book.SaveChanges();
                        }
                        // book.Carts.Add(new Cart { cid = Convert.ToInt32(userSettings), bid = Convert.ToInt32(ID[i]), });
                    }
                    //uncomment following if the above logic fails
                    //book.Orders.Add(new Order { cid = Convert.ToInt32(userSettings), orderdate = DateTime.Now, shippingaddress = ShippingAddress, Status = "Ordered", totalprice = total, orderid = n, qtysold = Convert.ToInt32(qty) });
                    //book.SaveChanges();
                }
                return RedirectToAction("Confirmation", "Home", new {orderNumber = n});
            }
            else {
                string userSettings = "0";
                if (Request.Cookies["UserSettings"] != null)
                {
                    if (Request.Cookies["UserSettings"]["UserID"] != null)
                    { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
                }
                using (BookStoreEntities _book = new BookStoreEntities())
                {
                    var userBooks = _book.Carts.Where(b => b.cid.ToString() == userSettings && b.inWishList == false && b.orderStatus == "Placed").ToList();
                    foreach (var b in userBooks)
                    {
                        _671Books.Models.Book book = new _671Books.Models.Book();
                        book.Author = b.author;
                        //book.BookCover
                        book.Edition = b.edition;
                        book.Genre = b.genre;
                        book.ID = b.bid;
                        //book.ISBN = b.i
                        book.Name = b.title;
                        book.Price = b.price;
                        book.Publisher = b.publisher;
                        book.QuantityInStore = b.qtyinstore;
                        book.Year = b.year;
                        _books.Add(book);
                    }

                }
                return View(_books);
            }

           
        }

        public ActionResult Confirmation(int OrderNumber)
        {
            ViewBag.orderNumber = OrderNumber;
            return View();
        }

        public ActionResult OrderHistory()
        {
            //List<_671Books.Models.Book> _books = new List<_671Books.Models.Book>();
            List<_671Books.Models.Orders> _orders = new List<_671Books.Models.Orders>();
            string userSettings = "";
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"]["UserID"] != null)
                { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
            }
            using (BookStoreEntities _book = new BookStoreEntities())
            {
                //var userBooks = _book.Orders.Where(b => b.cid.ToString() == userSettings).ToList();
                var userBooks = _book.GetOrderHistory().Where(b => b.cid.ToString() == userSettings).ToList();
                foreach (var b in userBooks)
                {
                    //_671Books.Models.Book book = new _671Books.Models.Book();
                    _671Books.Models.Orders order = new _671Books.Models.Orders();
                    
                    //book.Author = b.author;
                    //book.Edition = b.edition;
                    //book.Genre = b.genre;
                    //book.ID = b.bid;
                    
                    //book.Name = b.title;
                    //book.Price = b.price;
                    //book.Publisher = b.publisher;

                    //book.Year = b.year;
                    //_books.Add(book);
                    //order.ID = b.cid;
                    order.OrderID = b.orderid;
                    order.OrderDate = b.orderdate.ToShortDateString();
                    order.TotalPrice = b.totalprice;
                    order.ShippingAddress = b.shippingaddress;
                    order.Quantity = b.qtysold;
                    order.name = b.title;
                    order.publisher = b.publisher;
                    order.author = b.author;
                    order.genre = b.genre;
                    order.year = b.year;
                    order.edition = b.edition;
                    order.price = b.price;
                    order.Status = b.Status;
                    _orders.Add(order);
                }
            }
            return View(_orders);
        }

        [HttpPost]
        public ActionResult Checkout(FormCollection formCollection)
        {
            List<string> ID = new List<string>();
            List<string> a = new List<string>();
            int count = 0;
            string value = "";
            decimal total = 0;
            //string[] a = new string[100];
            string userSettings = "0";
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"]["UserID"] != null)
                { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
            }
            using(BookStoreEntities book = new BookStoreEntities())
            {
                foreach (var key in formCollection.AllKeys)
                {

                    if (key.Contains("txt"))
                    {
                         value = formCollection["txt"];
                        //a = value.Split(',');
                        
                    }
                    else
                    {
                        var v = key;                        
                        ID.Add(v);
                    }
                   
                    //var value = formCollection[key];
                    //var ID = key.Substring(2, key.Length - 3);
                    //if (key.Contains("txt"))
                    //{
                    //    var value = formCollection[key];
                    //    var ID = key.Substring(3, key.Length - 3);
                    //
                }
                 Random r = new Random();
                int n = r.Next(1000001, 9999999);

                string[] b = value.Split(',');
                for (int j = 0; j < b.Count(); j++)
                {
                    a.Add(b[j]);
                }
                var bookToRemove = book.Carts.Where(_book => _book.cid.ToString() == userSettings && _book.orderStatus == "Placed");
                book.Carts.RemoveRange(bookToRemove);
                book.SaveChanges();

                
                    for (int i = 0; i < a.Count(); i++)
                    {
                        var id = ID[i];
                        var originalRecord = book.books.FirstOrDefault(x => x.bid.ToString() ==  id);
                        if (originalRecord != null)
                        {
                            var num = Convert.ToDecimal(a[i]);
                            total = total + num;
                            var den = Convert.ToDecimal(originalRecord.price);
                            var qty = num / den;
                            //book.Carts.Add(new Cart { cid = Convert.ToInt32(userSettings), bid = originalRecord.bid, price = Convert.ToDecimal(a[i]), title = originalRecord.title, qty =  (int)qty, edition = originalRecord.edition, year = originalRecord.year, author = originalRecord.author, publisher = originalRecord.publisher, genre = originalRecord.genre,inWishList = false, orderStatus = "Ordered", qtyinstore = originalRecord.qtyinstore});
                            //book.SaveChanges();
                        }
                        // book.Carts.Add(new Cart { cid = Convert.ToInt32(userSettings), bid = Convert.ToInt32(ID[i]), });
                    }
                    book.Orders.Add(new Order { cid = Convert.ToInt32(userSettings), orderdate = DateTime.Now, shippingaddress = "123 Shipping Address", Status = "Ordered", totalprice = total, orderid = n });
                    book.SaveChanges();
            }
           
            return View();
 
        }

        [HttpPost]
        public ActionResult Confirmation(FormCollection formCollection)
        {
            Random r = new Random();
            int n = r.Next(1000001, 9999999);
            ViewBag.orderNumber = n;
            return View();
        }
        public ActionResult Admin()
        {
            BookStoreEntities book = new BookStoreEntities();


            List<_671Books.Models.Book> _books = new List<_671Books.Models.Book>();
            using (BookStoreEntities bookEntities = new BookStoreEntities())
            {
                //_books = bookEntities.Books.ToList();
                var books = bookEntities.books.Where(b => b.qtyinstore > 0).ToList();
                foreach (var b in books)
                {

                    _671Books.Models.Book _book = new _671Books.Models.Book();
                    _book.ID = b.bid;
                    _book.Name = b.title;
                    _book.Author = b.author;
                    _book.Genre = b.genre;
                    _book.Year = b.year;
                    _book.Edition = b.edition;
                    _book.Publisher = b.publisher;
                    _book.Price = b.price;
                    _book.QuantityInStore = b.qtyinstore;
                    //_book.QuantitySold = b.qtysold;
                    //_book.BookCover = b.BookCover;

                    _books.Add(_book);

                }
                return View(_books);

            }
        }

        [HttpPost]
        public ActionResult Admin(FormCollection formCollection)
        {
            List<_671Books.Models.Book> _books = new List<_671Books.Models.Book>();
            using (BookStoreEntities bookEntities = new BookStoreEntities())
            {

                var Author = formCollection["Author"];
                var Genre = (formCollection["Genres"]);
                var Year = (formCollection["Year"]);

                var books = bookEntities.books.ToList();

                if (Author.Length != 0)
                {
                    books = books.Where(b => b.author.ToLower().Contains(Author.ToLower()) && b.qtyinstore > 0).ToList();
                }
                if (Genre.Length != 0)
                {
                    books = books.Where(b => b.genre == Genre && b.qtyinstore > 0).ToList();
                }
                if (Year.Length != 0)
                {
                    books = books.Where(b => b.year.ToString() == Year && b.qtyinstore > 0).ToList();
                }
                foreach (var book in books)
                {
                    _671Books.Models.Book _book = new _671Books.Models.Book();
                    _book.ID = book.bid;
                    _book.Name = book.title;
                    _book.Author = book.author;
                    _book.Genre = book.genre;
                    _book.Year = book.year;
                    _book.Edition = book.edition;
                    _book.Publisher = book.publisher;
                    _book.Price = book.price;
                    _book.QuantityInStore = book.qtyinstore;
                    //_book.QuantitySold = book.qtysold;
                    _books.Add(_book);
                }
                return View(_books);
            }
        }

        public ActionResult NewBook()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult NewBook(FormCollection formCollection)
        {
            //var ID = Convert.ToInt32(formCollection["ID"]);
            
            var TitleBook = formCollection["TitleBook"];
            var Edition = formCollection["Edition"];
            var Year = formCollection["Year"];
            var Price = formCollection["Price"];
            var Author = formCollection["Author"];
            var Publisher = formCollection["Publisher"];
            var Genre = formCollection["Genre"];
            var QtyInStore = formCollection["QtyInStore"];
            decimal outPrice = 0;
            bool isPriceDecimal = Decimal.TryParse(Price, out outPrice);
            int outYear = 0;
            bool isYearNumerical = Int32.TryParse(Year, out outYear);
            int outQtyInStore = 0;
            bool isQtyInStoreNumerical = Int32.TryParse(QtyInStore, out outQtyInStore);
            int outEdition = 0;
            bool isEditionNumerical = Int32.TryParse(Edition, out outEdition);
            bool flag = true;
            bool numflag = true;
            string error = "";
            if (string.IsNullOrEmpty(TitleBook))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(Edition))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(Year))
            {
                flag = false;
            }
            if(string.IsNullOrEmpty(Author))
            {
                flag = false;
            }
             if(string.IsNullOrEmpty(Publisher))
            {
                flag = false;
            }
             if(string.IsNullOrEmpty(Genre))
            {
                flag = false;
            }
            if(string.IsNullOrEmpty(Price))
            {
                flag = false;
            }
             if(string.IsNullOrEmpty(QtyInStore))
            {
                flag = false;
            }
             if (!isPriceDecimal)
             {
                 numflag = false;
             }
             if (!isYearNumerical)
             {
                 numflag = false;
             }
             if (!isEditionNumerical)
             {
                 numflag = false;
             }
             if (!isQtyInStoreNumerical)
             {
                 numflag = false;
             }
             if (flag)
             {
                 if (numflag)
                 {
                     using (BookStoreEntities book = new BookStoreEntities())
                     {
                         var ID = book.books.Max(b => b.bid);
                         book.books.Add(new book { bid = ID + 1, title = TitleBook, edition = outEdition, author = Author, genre = Genre, price = outPrice, publisher = Publisher, year = outYear, qtyinstore = outQtyInStore});
                         book.SaveChanges();
                     }
                 }
                 else
                 {
                     ViewBag.SummaryDiv = "Please enter valid values";
                     return View();
                 }
             }
             else
             {
                 ViewBag.SummaryDiv = "Please enter all values";
                 return View();
             }
             return RedirectToAction("Admin", "Home");
            
        }

        public ActionResult BookDetailsAdmin(string ID)
        {
            _671Books.Models.Book _book = new _671Books.Models.Book();
            BookStoreEntities book = new BookStoreEntities();
            var books = book.books.ToList();
            books = books.Where(b => b.bid.ToString() == ID).ToList();
               
            foreach (var b in books)
            {
                _book.ID = b.bid;
                _book.Name = b.title;
                _book.Author = b.author;
                _book.Genre = b.genre;
                _book.Year = b.year;
                _book.Edition = b.edition;
                _book.Publisher = b.publisher;
                _book.Price = b.price;
                _book.QuantityInStore = b.qtyinstore;
                //_book.QuantitySold = b.qtysold;
                // _book.BookCover = b.BookCover;
            }
            return View(_book);
        }

        [HttpPost]
        public ActionResult BookDetailsAdmin(FormCollection formCollection)
        {
            var ID = Convert.ToInt32(formCollection["txtID"]);

            var TitleBook = formCollection["TitleBook"];
            var Edition = formCollection["Edition"];
            var Year = formCollection["Year"];
            var Price = formCollection["Price"];
            var Author = formCollection["Author"];
            var Publisher = formCollection["Publisher"];
            var Genre = formCollection["Genre"];
            var QtyInStore = formCollection["QtyInStore"];
            decimal outPrice = 0;
            bool isPriceDecimal = Decimal.TryParse(Price, out outPrice);
            int outYear = 0;
            bool isYearNumerical = Int32.TryParse(Year, out outYear);
            int outQtyInStore = 0;
            bool isQtyInStoreNumerical = Int32.TryParse(QtyInStore, out outQtyInStore);
            int outEdition = 0;
            bool isEditionNumerical = Int32.TryParse(Edition, out outEdition);
            bool flag = true;
            bool numflag = true;
            string error = "";
            if (string.IsNullOrEmpty(TitleBook))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(Edition))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(Year))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(Author))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(Publisher))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(Genre))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(Price))
            {
                flag = false;
            }
            if (string.IsNullOrEmpty(QtyInStore))
            {
                flag = false;
            }
            if (!isPriceDecimal)
            {
                numflag = false;
            }
            if (!isYearNumerical)
            {
                numflag = false;
            }
            if (!isEditionNumerical)
            {
                numflag = false;
            }
            if (!isQtyInStoreNumerical)
            {
                numflag = false;
            }
            if (flag)
            {
                if (numflag)
                {
                    using (BookStoreEntities book = new BookStoreEntities())
                    {
                        //var bookToRemove = book.books.Where(b => b.bid == ID);
                        //book.books.RemoveRange(bookToRemove);
                        //book.SaveChanges();

                        //book.books.Add(new book { bid = ID, title = TitleBook, edition = Edition, author = Author, genre = Genre, price = Price, publisher = Publisher, year = Year, qtyinstore = QtyInStore, qtysold = 0 });
                        //book.SaveChanges();
                        var bookToFind = book.books.Find(ID);
                        bookToFind.author = Author;
                        bookToFind.edition = outEdition;
                        bookToFind.year = outYear;
                        bookToFind.publisher = Publisher;
                        bookToFind.genre = Genre;
                        bookToFind.price = outPrice;
                        bookToFind.qtyinstore = outQtyInStore;
                        bookToFind.title = TitleBook;
                        book.SaveChanges();
                    }
                }
                else
                {
                    ViewBag.SummaryDiv = "Please enter valid values";
                    _671Books.Models.Book _book = new _671Books.Models.Book();
                    BookStoreEntities book = new BookStoreEntities();
                    var books = book.books.ToList();
                    books = books.Where(b => b.bid == ID).ToList();

                    foreach (var b in books)
                    {
                        _book.ID = b.bid;
                        _book.Name = b.title;
                        _book.Author = b.author;
                        _book.Genre = b.genre;
                        _book.Year = b.year;
                        _book.Edition = b.edition;
                        _book.Publisher = b.publisher;
                        _book.Price = b.price;
                        _book.QuantityInStore = b.qtyinstore;
                        //_book.QuantitySold = b.qtysold;
                        // _book.BookCover = b.BookCover;
                    }
                    return View(_book);
                }
            }
            else
            {
                ViewBag.SummaryDiv = "Please enter all values";
                _671Books.Models.Book _book = new _671Books.Models.Book();
                BookStoreEntities book = new BookStoreEntities();
                var books = book.books.ToList();
                books = books.Where(b => b.bid == ID).ToList();

                foreach (var b in books)
                {
                    _book.ID = b.bid;
                    _book.Name = b.title;
                    _book.Author = b.author;
                    _book.Genre = b.genre;
                    _book.Year = b.year;
                    _book.Edition = b.edition;
                    _book.Publisher = b.publisher;
                    _book.Price = b.price;
                    _book.QuantityInStore = b.qtyinstore;
                    //_book.QuantitySold = b.qtysold;
                    // _book.BookCover = b.BookCover;
                }
                return View(_book);
            }
           
            
           
            return RedirectToAction("Admin", "Home");
        }

        public ActionResult TotalSales()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TotalSales(FormCollection formCollection)
        {
            var fromdate = Convert.ToDateTime(formCollection["fromdate"]);
            var todate = Convert.ToDateTime(formCollection["todate"]);
            
            ViewBag.fromdate = fromdate;
            ViewBag.todate = todate;
            List<_671Books.Models.Orders> orders = new List<_671Books.Models.Orders>();
            using (BookStoreEntities book = new BookStoreEntities())
            {
                //var order = book.Orders.Where(o => o.orderdate >= fromdate && o.orderdate <= todate).ToList();
                var order = book.GetOrderHistory().Where(o => o.orderdate >= fromdate && o.orderdate <= todate).ToList();
                ViewBag.Orders = "orders";
                foreach (var o in order)
                {
                    _671Books.Models.Orders _order = new _671Books.Models.Orders();
                    _order.OrderID = o.orderid;
                    _order.OrderDate = o.orderdate.ToShortDateString();
                    _order.TotalPrice = o.totalprice;
                    _order.name = o.title;
                    _order.publisher = o.publisher;
                    _order.genre = o.genre;
                    _order.edition = o.edition;
                    _order.author = o.author;
                    _order.year = o.year;

                    orders.Add(_order);
                }
            }
            return View(orders);
        }

        public ActionResult MyAccount()
        {
            List<_671Books.Models.Person> _people = new List<_671Books.Models.Person>();
             string userSettings = "0";
            if (Request.Cookies["UserSettings"] != null)
            {
                if (Request.Cookies["UserSettings"]["UserID"] != null)
                { userSettings = Request.Cookies["UserSettings"]["UserID"]; }
            }
            using (BookStoreEntities book = new BookStoreEntities())
            {
                var person = book.people.Where(p => p.cid.ToString() == userSettings).ToList();
                foreach (var p in person)
                {
                    _671Books.Models.Person _person = new _671Books.Models.Person();
                    _person.ID = p.cid;
                    _person.Name = p.name;
                    _person.Phone = p.phone;
                    _person.Address = p.address;
                    _person.UserName = p.username;
                    _person.Password = p.password;

                    _people.Add(_person);
                }
                return View(_people);

            }
            
        }
    }
}
