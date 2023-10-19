namespace EquatableApp;

[CodeGenerationSample.ImplementEquatable]
public partial class Book
{
    public Book(string title, string publisher)
    {
        Title = title;
        Publisher = publisher;
    }
    public string Title { get; }
    public string Publisher { get; }

    //在 C# 9中，分部方法可以返回类型。这个分部方法需要有一个私有访问修饰符以及一个实现
    private static partial bool IsTheSame(Book? left, Book? right) =>
           left?.Title == right?.Title && left?.Publisher == right?.Publisher;

    public override int GetHashCode() =>
         Title.GetHashCode() ^ Publisher.GetHashCode();
}
