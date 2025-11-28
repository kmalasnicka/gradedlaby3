using System;
using System.Reflection;
using System.Globalization;

namespace TypeCrafter;
public class ParseException : Exception //custom exception type for parsing errors, which inherits from Exception
{//three constructors
    public ParseException() { }//one parameterless

    public ParseException(string message) //one with a message 
        : base(message) { }

    public ParseException(string message, Exception innerException) //one with message + inner exception
        : base(message, innerException) { }
}

public static class TypeCrafter//static because we dont create an instance of it
{
    public static string AskForInput(string propertyName, string typeName){
            Console.WriteLine($"Provide a value of type {typeName} for property {propertyName}:"); //prompt to console
            string? line = Console.ReadLine(); //reads line from user using Console.Readline()
            if(line != null) return line; //if input is not null we return the line
            throw new IOException("Line from the Console is not available.");
    }

    public static T CraftInstance<T>() // generic method that returns object of type T and builds it dynamically using reflection and console input
    {
        var type = typeof(T); //instance of type T, start point for reflection from type we get constructors, properties, methods etc
        //T must have parameterless constructor:
        var constructor = type.GetConstructor(Type.EmptyTypes) //serches for a public parameterless constructor, if there is no such we return null
            ?? throw new InvalidOperationException($"Type {type.FullName} has no parameterless constructor."); //if null throw exception
        
        var result = (T)constructor.Invoke(null); // create object, Invoke(null) because constructor in parameterless, casts the created object to T
        //result is now an empty/uninitialized instance of type T

        var properties = type.GetProperties(); // gets all public properties of type T, returns array of PropertyInfo
        foreach(var property in properties){
            if(!property.CanWrite) continue; //checks if the property has a setter, if its read-only we skip
            var propertyType = property.PropertyType; // this gives Type of property
            if(propertyType == typeof(string)){ // case if property is a string
                var input = AskForInput(property.Name, propertyType.Name); // we ask user for values
                property.SetValue(result, input); //sets the property on the result object the user entered
                continue; //move to next property
            }
            //next labs:
            var isParsable = propertyType.GetInterfaces()
            .Any(t => t.IsGenericType && //t must be generic
            t.GetGenericTypeDefinition() == typeof(IParsable<>) && // generic interface must be IParsable<>
            t.GetGenericArguments()[0] == propertyType); // gets array of types, we make sure first is propertyType, because it has only one argument

            if(isParsable){ //if the type is parsable
                var input = AskForInput(property.Name, propertyType.Name); //ask for user input
                var parseMethod = propertyType.GetMethod( //locating tryParse method
                "TryParse",
                BindingFlags.Public | BindingFlags.Static,
                binder: null,
                types: new[] {
                typeof(string), 
                typeof(IFormatProvider), 
                propertyType.MakeByRefType()
            },
                modifiers: null
            ) ?? throw new Exception($"{propertyType.FullName} does not havre a static TryParse method"); //if no such method exists
                var args = new object[] {input, null! , null!}; //preparing arguments for parsing, args[0] - input, args[1] - format provider, args[2] - will be filled by TryParse with the parsed value
                if(parseMethod.Invoke(null, args) is bool status && status){ //null because its a static method not called on an instance, args[2] contains parsed value, method Invoke returns bool, we assign it to status
                    property.SetValue(result, args[2]); //on success we set the property to parsed value
                } else {
                    throw new ParseException(); //on failure
                }
                continue;
            }
            //if both cases fail
            Console.WriteLine($"Type of property '{property.PropertyType} {property.Name}' is not parsable.");
            Console.WriteLine("Attempting to craft object recursively:");
            //recursively calls craftinstance for nested type
            var craftMethod = typeof(TypeCrafter)
            .GetMethod(nameof(CraftInstance), //get MethodInfo for CraftInstance on TypeCrafter, nameof(CraftInstance) avoids writing the method name as a string
            BindingFlags.Public | BindingFlags.Static);
            var genericMethod = craftMethod!.MakeGenericMethod(property.PropertyType);
            var complexProperty = genericMethod.Invoke(null, null);

            property.SetValue(result, complexProperty);
        }
        return result;
    }
}