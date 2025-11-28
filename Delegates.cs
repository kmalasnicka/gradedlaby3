// delegates - object that knows how to call a method, they allow to hold references to methods
// delegate can call methods of same type of parameters taken and return type, 

// delegate double Function(double x); // Declaration of delegate type

double QuadraticFunction(double x) => x * x - 2 * x + 1; // A compatible method

Function quadratic = QuadraticFunction; // Create a delegate instance

double y = quadratic(2.0); // inovke delegate, equivalent to: quadratic.Invoke(2.0)

delegate T Function<T>(T x); // generic delegate type

//+= and -= operators allow adding and removing methods from a delegate