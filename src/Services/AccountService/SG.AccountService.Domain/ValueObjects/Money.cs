namespace SG.AccountService.Domain.ValueObjects;

using SG.AccountService.Domain.Exceptions; // Asumiendo que tenés tus excepciones acá

public record Money
{
  public decimal Value { get; }

  public Money(decimal value)
  {
    if (value < 0)
      throw new InvalidMoneyException("El monto no puede ser negativo.");
    
    if (decimal.Round(value, 2) != value)
      throw new InvalidMoneyException("El monto no puede tener más de dos decimales.");
    
    Value = value;
  }
  
  public static implicit operator decimal(Money money) => money.Value;
  public static readonly Money Zero = new(0);
  public static Money operator +(Money a, Money b) => new Money(a.Value + b.Value);
  public static Money operator -(Money a, Money b) => new Money(a.Value - b.Value);
}