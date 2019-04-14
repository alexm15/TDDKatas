using System;
using System.Collections.Generic;
using Xunit;

namespace TDDKatas
{
    public class MoneyExampleTests
    {
        [Fact]
        public void TestMultiplication()
        {
            Money five = Money.Dollar(5);
            Assert.Equal(Money.Dollar(10), five.Times(2));
            Assert.Equal(Money.Dollar(15), five.Times(3));
        }

        [Fact]
        public void TestEquality()
        {
            Assert.True(Money.Dollar(5).Equals(Money.Dollar(5)));
            Assert.False(Money.Dollar(5).Equals(Money.Dollar(6)));
            Assert.False(Money.Dollar(5).Equals(Money.Franc(5)));
        }


        [Fact]
        public void TestCurrency()
        {
            Assert.Equal("USD", Money.Dollar(1).Currency());
            Assert.Equal("CHF", Money.Franc(1).Currency());
        }


        /*abc
        $5 + 10 CHF = $10 if rate is 2:1
        Money rounding?
        hashCode()
        Equal null
        Equal object
        */
    }

    class Money
    {
        protected int amount;
        protected string currency;

        public Money(int amount, string currency)
        {
            this.amount = amount;
            this.currency = currency;

        }


        public Money Times(int multiplier)
        {
            return new Money(amount * multiplier, currency);
        }

        public override bool Equals(object obj)
        {
            Money money = (Money)obj;
            return amount == money.amount
                && Currency().Equals(money.Currency());
        }

        public static Money Dollar(int amount)
        {
            return new Money(amount, "USD");
        }

        public static Money Franc(int amount)
        {
            return new Money(amount, "CHF");
        }

        public string Currency()
        {
            return currency;
        }

        public override string ToString()
        {
            return amount + " " + currency;
        }
    }
}