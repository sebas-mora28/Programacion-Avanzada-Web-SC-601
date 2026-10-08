class Bird
{
    public virtual void Fly()
    {
        Console.WriteLine("El ave está volando");
    }
}


class Eagle : Bird
{
    public override void Fly()
    {
        Console.WriteLine("El águila está volando");
    }
}

class Sparrow : Bird
{
    public override void Fly()
    {
        Console.WriteLine("El gorrión está volando");
    }
}


class Penguin : Bird
{
    public override void Fly()
    {
        throw new NotSupportedException(
            "Los pingüinos no pueden volar"
        );
    }
}


// #################################


abstract class Bird
{
    public void Eat()
    {
        Console.WriteLine("El ave está comiendo");
    }
}


abstract class FlyingBird : Bird
{
    public abstract void Fly();
}


class Eagle : FlyingBird
{
    public override void Fly()
    {
        Console.WriteLine("El águila está volando");
    }
}

class Sparrow : FlyingBird
{
    public override void Fly()
    {
        Console.WriteLine("El gorrión está volando");
    }
}

class Penguin : Bird
{
    public void Swim()
    {
        Console.WriteLine("El pingüino está nadando");
    }
}


