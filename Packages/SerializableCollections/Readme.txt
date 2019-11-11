Serializable Collections for Unity gives you versions of all the collections in
the .NET System.Collections.Generic namespace. Each of these collections is, as
nearly as possible, a 1:1 version of the collection it's based on; meaning that
you can work with them exactly as you would the normal collection.

Installation:

Just install the package, and you're done! All of the collections can be found
in the SerializableCollections namespace.

Usage:

Mostly, you can use these collections exactly as they're documented on the MSDN
web page (https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic).
The only major difference is that, due to the way Unity handles serialization,
you need to declare a concrete class which makes use of these generics. So,
for example, rather than doing this:

private List<string> listOfStrings = new List<string>();

You would do this:

[Serializable]
public class StringList : List<string> {}
private StringList listOfStrings = new StringList();

You can create your classes inside the class you're working in, or you can
create them in their own file for the rest of your program to take advantage
of.