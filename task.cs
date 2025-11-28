using System;
using System.Linq;
using System.Reflection;
using System.Globalization;

//creating own attributes
[AttributeUsage(AttributeTargets.Class)] //this attriute can be applied only to classes
public class MyClassAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Property)] //this attribute can be applied only to properties
public class MyPropertyAttribute : Attribute {}

public abstract class Animal{ // base class
    public abstract void Speak();
}

//test classes
[MyClassAttribute]
public class Dog : Animal, IParsable<Dog>{ // Dog inherits from Animal, IParsable<Dog> implements generic interface saying "dog knows how to parse itself from string"
    [MyPropertyAttribute]
    public string Name {get; set;} = "";

    public int Age {get; set;}

    public override void Speak() => Console.WriteLine("Woof!");

    //method required by IParsable<Dog>
    public static bool TryParse(string s, IFormatProvider provider, out Dog result){ // takes input text, culture / formatting info (not used here), parsed result
        result = new Dog() {Name = s}; // creates a new Dog with Name = s
        return true; // if success
    }
}

public class Cat : Animal{ // inherits from Animal
    public string Color {get; set;} = "";
    public override void Speak() => Console.WriteLine("Meow!");
}

public class Car{
    [MyPropertyAttribute]
    public string Model {get; set;} = "";

    public int Horsepower {get; set;}
}

class Program{
    static void Main(){
        Assembly assembly = Assembly.GetExecutingAssembly(); //Assembly.GetExecutingAssembly() gets the assembly where this code runs
        Console.WriteLine("Types marked with MyClassAttribute");
        var markedTypes =
            assembly.GetTypes() //returns all types defined in this assembly
            .Where(t => t.GetCustomAttribute<MyClassAttribute>() != null); // for each type checks if[MyClassAttribute] was applied, GetCustomAttribute<MyClassAttribute>() returns the attribute instance or null
            
        foreach(var type in markedTypes){
            Console.WriteLine(type.Name);
        }

        Console.WriteLine("Types inheriting from abstract class Animal");
        var animals = 
            assembly.GetTypes().Where(t => 
            t.IsSubclassOf(typeof(Animal))); //true if t is a class that inherits from Animal

        foreach(var animal in animals){
            Console.WriteLine(animal.Name);
        }

        Console.WriteLine("Properties marked with MyPropertyAttriubute");
        var allTypes = assembly.GetTypes();
        foreach(var t in allTypes){
            var props = t.GetProperties() //gets all public properties
                .Where(p => p.GetCustomAttribute<MyPropertyAttribute>() != null); // keeps only those with [MyPropertyAttribute]

            foreach(var p in props){
                Console.WriteLine($"{t.Name}.{p.Name}");
            }
        }

        Console.WriteLine("Checking if a type implements IParsable<T>");
        Type parsable = typeof(IParsable<>);
        foreach(var t in allTypes){
            bool implementsParsable =
                t.GetInterfaces().Any(i => //all interfaces it implements
                i.IsGenericType && //check it’s a generic interface
                i.GetGenericTypeDefinition() == parsable && //check the generic definition is IParsable<>
                i.GetGenericArguments()[0] == t); //for IParsable<Dog> → GetGenericArguments()[0] is Dog
            if(implementsParsable)
                Console.WriteLine($"{t.Name} implements IParsable<{t.Name}>");
        }

        Console.WriteLine("Setting property value dynamically using SetValue");
        var car = new Car(); // create car object
        PropertyInfo horsepowerProp = typeof(Car).GetProperty("Horsepower")!; //uses reflection to get the PropertyInfo for the property named "Horsepower", ! - its not null
        horsepowerProp.SetValue(car, 150); //sets car.Horsepower to 150 using reflection instead of car.Horsepower = 150

        Console.WriteLine($"Car horsepower set via reflection: {car.Horsepower}");
    }
}