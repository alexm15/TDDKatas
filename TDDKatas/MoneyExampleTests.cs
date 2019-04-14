using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

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

        [Fact]
        public void TestSimpleAddition()
        {
            Money five = Money.Dollar(5);
            MoneyExpression sum = five.Plus(five);
            Bank bank = new Bank();
            Money reduced = bank.Reduce(sum, "USD");
            Assert.Equal(Money.Dollar(10), reduced);
        }

        [Fact]
        public void TestReduceSum()
        {
            MoneyExpression sum = new Sum(Money.Dollar(3), Money.Dollar(4));
            Bank bank = new Bank();
            Money reduced = bank.Reduce(sum, "USD");
            Assert.Equal(Money.Dollar(7), reduced);
        }

        [Fact]
        public void TestReduceMoney()
        {
            Bank bank = new Bank();
            bank.AddRate("CHF", "USD", 2);
            Money result = bank.Reduce(Money.Dollar(1), "USD");
            Assert.Equal(Money.Dollar(1), result);
        }

        [Fact]
        public void TestReduceMoneyDifferentCurrency()
        {
            Bank bank = new Bank();
            bank.AddRate("CHF", "USD", 2);
            Money result = bank.Reduce(Money.Franc(2), "USD");
            Assert.Equal(Money.Dollar(1), result);
        }

        [Fact]
        public void TestPlusReturnsSum()
        {
            Money five = Money.Dollar(5);
            MoneyExpression result = five.Plus(five);
            Sum sum = (Sum) result;
            Assert.Equal(five, sum.augend);
            Assert.Equal(five, sum.addend);
        }

        [Fact]
        public void TestIdentityRate()
        {
            Assert.Equal(1, new Bank().Rate("USD", "USD"));
        }

        [Fact]
        public void TestMixedAddition()
        {
            MoneyExpression fiveBucks = Money.Dollar(5);
            MoneyExpression tenFrancs = Money.Franc(10);
            Bank bank = new Bank();
            bank.AddRate("CHF", "USD", 2);
            Money result = bank.Reduce(fiveBucks.Plus(tenFrancs), "USD");
            Assert.Equal(Money.Dollar(10), result);
        }

        [Fact]
        public void TestSumPlusMoney()
        {
            MoneyExpression fiveBucks = Money.Dollar(5);
            MoneyExpression tenFrancs = Money.Franc(10);
            Bank bank = new Bank();
            bank.AddRate("CHF", "USD", 2);
            MoneyExpression sum = new Sum(fiveBucks, tenFrancs).Plus(fiveBucks);
            Money result = bank.Reduce(sum, "USD");
            Assert.Equal(Money.Dollar(15), result);
        }

        [Fact]
        public void TestSumTimes()
        {
            MoneyExpression fiveBucks = Money.Dollar(5);
            MoneyExpression tenFrancs = Money.Franc(10);
            Bank bank = new Bank();
            bank.AddRate("CHF", "USD", 2);
            MoneyExpression sum = new Sum(fiveBucks, tenFrancs).Times(2);
            Money result = bank.Reduce(sum, "USD");
            Assert.Equal(Money.Dollar(20), result);
        }



        /*
        Duplication between Sum.Plus & Money.Plus 
        */
    }

    class Sum : MoneyExpression
    {
        public MoneyExpression augend;
        public MoneyExpression addend;

        public Sum(MoneyExpression augend, MoneyExpression addend)
        {
            this.augend = augend;
            this.addend = addend;
        }

        public override Money Reduce(Bank bank, string to)
        {
            int amount = augend.Reduce(bank, to).amount + addend.Reduce(bank, to).amount;
            return new Money(amount, to);
        }

        public override MoneyExpression Times(int multiplier)
        {
            return new Sum(augend.Times(multiplier), addend.Times(multiplier));
        }
    }

    class Bank
    {
        private Hashtable rates = new Hashtable();
        public Money Reduce(MoneyExpression source, string to)
        {
            return source.Reduce(this, to);
        }

        public void AddRate(string from, string to, int rate)
        {
            rates.Add(new Pair(from, to), rate);
        }

        public int Rate(string from, string to)
        {
            if (from.Equals(to)) return 1;
            int rate = (int) rates[new Pair(from, to)];
            return rate;
            
        }

        private class Pair
        {
            private string from;
            private string to;

            public Pair(string from, string to)
            {
                this.from = from;
                this.to = to;
            }

            public override bool Equals(object obj)
            {
                var pair = (Pair) obj;
                return from.Equals(pair.from) && to.Equals(pair.to);
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }
    }

    abstract class MoneyExpression
    {
        public abstract Money Reduce(Bank bank, string to);

        public MoneyExpression Plus(MoneyExpression addend)
        {
            return new Sum(this, addend);
        }

        public abstract MoneyExpression Times(int multiplier);
    }

    class Money : MoneyExpression
    {
        public int amount;
        private string currency;

        public Money(int amount, string currency)
        {
            this.amount = amount;
            this.currency = currency;
        }

        public override MoneyExpression Times(int multiplier)
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

        

        public override Money Reduce(Bank bank, string to)
        {
            int rate = bank.Rate(currency, to);
            return new Money(amount / rate, to);
        }
    }
}