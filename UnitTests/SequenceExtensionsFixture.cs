﻿using System;
using System.Threading.Tasks;
using Xunit;

namespace Moq.Tests
{
	public class SequenceExtensionsFixture
	{
		[Fact]
		public void PerformSequence()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.Do())
				.Returns(2)
				.Returns(3)
				.Throws<InvalidOperationException>();

			Assert.Equal(2, mock.Object.Do());
			Assert.Equal(3, mock.Object.Do());
			Assert.Throws<InvalidOperationException>(() => mock.Object.Do());
		}

		[Fact]
		public void PerformSequenceAsync()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.DoAsync())
				.ReturnsAsync(2)
				.ReturnsAsync(3)
				.ThrowsAsync(new InvalidOperationException());

			Assert.Equal(2, mock.Object.DoAsync().Result);
			Assert.Equal(3, mock.Object.DoAsync().Result);

			try
			{
				var x = mock.Object.DoAsync().Result;
			}
			catch (AggregateException ex)
			{
				Assert.IsType<InvalidOperationException>(ex.GetBaseException());
			}
		}

		[Fact]
		public void PerformSequenceOnProperty()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.Value)
				.Returns("foo")
				.Returns("bar")
				.Throws<SystemException>();

			string temp;
			Assert.Equal("foo", mock.Object.Value);
			Assert.Equal("bar", mock.Object.Value);
			Assert.Throws<SystemException>(() => temp = mock.Object.Value);
		}

		[Fact]
		public void PerformSequenceWithThrowsFirst()
		{
			var mock = new Mock<IFoo>();

			mock.SetupSequence(x => x.Do())
				.Throws<Exception>()
				.Returns(1);

			Assert.Throws<Exception>(() => mock.Object.Do());
			Assert.Equal(1, mock.Object.Do());
		}

		[Fact]
		public void PerformSequenceWithCallBase()
		{
			var mock = new Mock<Foo>();

			mock.SetupSequence(x => x.Do())
				.Returns("Good")
				.CallBase()
				.Throws<InvalidOperationException>();

			Assert.Equal("Good", mock.Object.Do());
			Assert.Equal("Ok", mock.Object.Do());
			Assert.Throws<InvalidOperationException>(() => mock.Object.Do());
		}

		public interface IFoo
		{
			string Value { get; set; }

			int Do();

			Task<int> DoAsync();
		}

		public class Foo
		{
			public virtual string Do()
			{
				return "Ok";
			}

			public virtual Task<string> DoAsync()
			{
				var tcs = new TaskCompletionSource<string>();
				tcs.SetResult("Ok");
				return tcs.Task;
			}
		}
	}
}