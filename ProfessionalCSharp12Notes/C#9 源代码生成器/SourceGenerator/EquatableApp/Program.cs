﻿using EquatableApp;

Book b1 = new("Professional C#", "Wrox Press");
Book b2 = new("Professional C#", "Wrox Press");
if (b1 == b2)
{
    Console.WriteLine("the same book");
}
else
{
    Console.WriteLine("not the same book");
}