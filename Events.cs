using System;

//event (delegate with extra encapsulation coming from event keyword)- allows a class or object to notify other classes or objects about the occurrence of a specific action, event keyword allows only addition and removal of subscribes
// EventArgs class - stores information about the event
// Publisher - class that declares and raises the event
// Subscribers - classes that react to the event, can attach or detach to it
public class PriceChangedEventArgs : EventArgs{
    public decimal NewPrice {get;}
    public decimal OldPrice {get;}

    public PriceChangedEventArgs(decimal newPrice, decimal oldPrice){
        NewPrice = newPrice;
        OldPrice = oldPrice;
    }
}

public class Product
{ // because its an event external code cannod do anything to it they can only subscribe or unsubscribe 
    public event EventHandler<PriceChangedEventArgs>? PriceChanged; // ? means it can be null, we declare event with event keyword, PriceChanged is event name
//Type of the event: EventHandler<PriceChangedEventArgs> (a delegate type).
    public required string Name { get; init; }
    private decimal _price;
    public decimal Price 
    {
        get => _price;
        set
        {
            if (value == _price) return; // if same nothing happens
            OnPriceChanged(new PriceChangedEventArgs(_price, value)); // if not same we call OnPriceChganges
            _price = value;
        }
    }
// protected virtual allows subclasses of Product to override how and when they raise the event
    protected virtual void OnPriceChanged(PriceChangedEventArgs e) // publisher
    {//invoke executed all stored methods, subscriber is stored inside event delegate
        PriceChanged?.Invoke(this, e); // ? ensures PriceChanged isnt null if it is noting happens, this is a Product instance, e is event data object
    }
}

public class Notifier
{
    // Event handler, with matching signature:
    public void HandlePriceChanged(object sender, PriceChangedEventArgs e) // subscriber
    {
        if (sender is Product product)
        {
            Console.WriteLine($"Price of {product.Name} changed. {e.OldPrice:C} -> {e.NewPrice:C}");
        }
    }
}

var product = new Product { Name = "Laptop", Price = 1199.90m };
var notifier = new Notifier();

product.PriceChanged += notifier.HandlePriceChanged;

// later, to unsubscribe:
product.PriceChanged -= notifier.HandlePriceChanged;

//When the event fires, the publisher (Product) calls PriceChanged.Invoke(this, e). That causes all subscribed handlers to run, including notifier.HandlePriceChanged