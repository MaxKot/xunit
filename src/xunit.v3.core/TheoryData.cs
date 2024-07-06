using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit.Internal;

namespace Xunit;

/// <summary>
/// Provides data for theories based on collection initialization syntax.
/// </summary>
public abstract class TheoryData : IReadOnlyCollection<ITheoryDataRow>
{
	readonly List<ITheoryDataRow> data = [];

	/// <inheritdoc/>
	public int Count => data.Count;

	/// <summary>
	/// Adds a row to the theory.
	/// </summary>
	/// <param name="values">The values to be added.</param>
	protected void AddRow(params object?[] values) =>
		AddRow(new TheoryDataRow(values));

	/// <summary>
	/// Adds a row to the theory.
	/// </summary>
	/// <param name="dataRow">The row of data to be added.</param>
	protected void AddRow(ITheoryDataRow dataRow)
	{
		Guard.ArgumentNotNull(dataRow);

		data.Add(dataRow);
	}

	/// <summary>
	/// Adds multiple rows to the theory.
	/// </summary>
	/// <param name="rows">The rows to be added.</param>
	protected void AddRows(IEnumerable<object?[]> rows)
	{
		Guard.ArgumentNotNull(rows);

		foreach (var row in rows)
			AddRow(row);
	}

	/// <summary>
	/// Adds multiple rows to the theory.
	/// </summary>
	/// <param name="rows">The rows to be added.</param>
	protected void AddRows(IEnumerable<ITheoryDataRow> rows)
	{
		Guard.ArgumentNotNull(rows);

		foreach (var row in rows)
			AddRow(row);
	}

	/// <inheritdoc/>
	public IEnumerator<ITheoryDataRow> GetEnumerator() => data.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Represents a set of data for a theory with a single parameter. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T">The parameter type.</typeparam>
public class TheoryData<T> : TheoryData, IReadOnlyCollection<T?>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<T> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params T[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p">The data value.</param>
	public void Add(T p) =>
		AddRow(p);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p">The data value.</param>
	public void Add(TheoryDataRow<T> p) =>
		AddRow(p);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params T[] values) =>
		AddRows(values.Select(x => new object?[] { x }));

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T>[] values) =>
		AddRows(values.Select(x => new object?[] { x }));

	/// <inheritdoc />
	public new IEnumerator<T?> GetEnumerator()
	{
		foreach(var row in (IEnumerable<ITheoryDataRow>) this)
		{
			var data = row.GetData();
			yield return (T?)data[0];
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 2 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
public class TheoryData<T1, T2> : TheoryData, IReadOnlyCollection<(T1?, T2?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	public void Add(T1 p1, T2 p2) =>
		AddRow(p1, p2);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2) row) =>
		Add(row.Item1, row.Item2);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2> row) =>
		AddRow(row);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>) this)
		{
			var data = row.GetData();
			yield return ((T1?)data[0], (T2?)data[1]);
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 3 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
/// <typeparam name="T3">The third parameter type.</typeparam>
public class TheoryData<T1, T2, T3> : TheoryData, IReadOnlyCollection<(T1?, T2?, T3?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2, T3)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2, T3)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2, T3>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	/// <param name="p3">The third data value.</param>
	public void Add(T1 p1, T2 p2, T3 p3) =>
		AddRow(p1, p2, p3);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2, T3> row) =>
		AddRow(row);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2, T3) row) =>
		Add(row.Item1, row.Item2, row.Item3);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2, T3 p3)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2, x.p3 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2, T3>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?, T3?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>)this)
		{
			var data = row.GetData();
			Debug.Assert(data != null);
			yield return ((T1?)data[0], (T2?)data[1], (T3?)data[2]);
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 4 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
/// <typeparam name="T3">The third parameter type.</typeparam>
/// <typeparam name="T4">The fourth parameter type.</typeparam>
public class TheoryData<T1, T2, T3, T4> : TheoryData, IReadOnlyCollection<(T1?, T2?, T3?, T4?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2, T3, T4)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2, T3, T4)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2, T3, T4>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	/// <param name="p3">The third data value.</param>
	/// <param name="p4">The fourth data value.</param>
	public void Add(T1 p1, T2 p2, T3 p3, T4 p4) =>
		AddRow(p1, p2, p3, p4);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2, T3, T4) row) =>
		Add(row.Item1, row.Item2, row.Item3, row.Item4);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2, T3, T4> row) =>
		AddRow(row);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2, T3 p3, T4 p4)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2, x.p3, x.p4 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2, T3, T4>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?, T3?, T4?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>)this)
		{
			var data = row.GetData();
			Debug.Assert(data != null);
			yield return ((T1?)data[0], (T2?)data[1], (T3?)data[2], (T4?)data[3]);
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 5 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
/// <typeparam name="T3">The third parameter type.</typeparam>
/// <typeparam name="T4">The fourth parameter type.</typeparam>
/// <typeparam name="T5">The fifth parameter type.</typeparam>
public class TheoryData<T1, T2, T3, T4, T5> : TheoryData, IReadOnlyCollection<(T1?, T2?, T3?, T4?, T5?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2, T3, T4, T5)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2, T3, T4, T5)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2, T3, T4, T5>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	/// <param name="p3">The third data value.</param>
	/// <param name="p4">The fourth data value.</param>
	/// <param name="p5">The fifth data value.</param>
	public void Add(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) =>
		AddRow(p1, p2, p3, p4, p5);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2, T3, T4, T5) row) =>
		Add(row.Item1, row.Item2, row.Item3, row.Item4, row.Item5);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2, T3, T4, T5> row) =>
		AddRow(row);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2, x.p3, x.p4, x.p5 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2, T3, T4, T5>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?, T3?, T4?, T5?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>)this)
		{
			var data = row.GetData();
			Debug.Assert(data != null);
			yield return ((T1?)data[0], (T2?)data[1], (T3?)data[2], (T4?)data[3], (T5?)data[4]);
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 6 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
/// <typeparam name="T3">The third parameter type.</typeparam>
/// <typeparam name="T4">The fourth parameter type.</typeparam>
/// <typeparam name="T5">The fifth parameter type.</typeparam>
/// <typeparam name="T6">The sixth parameter type.</typeparam>
public class TheoryData<T1, T2, T3, T4, T5, T6> : TheoryData, IReadOnlyCollection<(T1?, T2?, T3?, T4?, T5?, T6?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2, T3, T4, T5, T6)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2, T3, T4, T5, T6)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2, T3, T4, T5, T6>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	/// <param name="p3">The third data value.</param>
	/// <param name="p4">The fourth data value.</param>
	/// <param name="p5">The fifth data value.</param>
	/// <param name="p6">The sixth data value.</param>
	public void Add(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6) =>
		AddRow(p1, p2, p3, p4, p5, p6);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2, T3, T4, T5, T6) row) =>
		Add(row.Item1, row.Item2, row.Item3, row.Item4, row.Item5, row.Item6);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2, T3, T4, T5, T6> row) =>
		AddRow(row);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2, x.p3, x.p4, x.p5, x.p6 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2, T3, T4, T5, T6>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?, T3?, T4?, T5?, T6?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>)this)
		{
			var data = row.GetData();
			Debug.Assert(data != null);
			yield return ((T1?)data[0], (T2?)data[1], (T3?)data[2], (T4?)data[3], (T5?)data[4], (T6?)data[5]);
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 7 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
/// <typeparam name="T3">The third parameter type.</typeparam>
/// <typeparam name="T4">The fourth parameter type.</typeparam>
/// <typeparam name="T5">The fifth parameter type.</typeparam>
/// <typeparam name="T6">The sixth parameter type.</typeparam>
/// <typeparam name="T7">The seventh parameter type.</typeparam>
public class TheoryData<T1, T2, T3, T4, T5, T6, T7> : TheoryData, IReadOnlyCollection<(T1?, T2?, T3?, T4?, T5?, T6?, T7?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2, T3, T4, T5, T6, T7)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2, T3, T4, T5, T6, T7)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2, T3, T4, T5, T6, T7>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	/// <param name="p3">The third data value.</param>
	/// <param name="p4">The fourth data value.</param>
	/// <param name="p5">The fifth data value.</param>
	/// <param name="p6">The sixth data value.</param>
	/// <param name="p7">The seventh data value.</param>
	public void Add(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7) =>
		AddRow(p1, p2, p3, p4, p5, p6, p7);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2, T3, T4, T5, T6, T7) row) =>
		Add(row.Item1, row.Item2, row.Item3, row.Item4, row.Item5, row.Item6, row.Item7);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2, T3, T4, T5, T6, T7> row) =>
		AddRow(row);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2, x.p3, x.p4, x.p5, x.p6, x.p7 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2, T3, T4, T5, T6, T7>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?, T3?, T4?, T5?, T6?, T7?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>)this)
		{
			var data = row.GetData();
			Debug.Assert(data != null);
			yield return ((T1?)data[0], (T2?)data[1], (T3?)data[2], (T4?)data[3], (T5?)data[4], (T6?)data[5], (T7?)data[6]);
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 8 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
/// <typeparam name="T3">The third parameter type.</typeparam>
/// <typeparam name="T4">The fourth parameter type.</typeparam>
/// <typeparam name="T5">The fifth parameter type.</typeparam>
/// <typeparam name="T6">The sixth parameter type.</typeparam>
/// <typeparam name="T7">The seventh parameter type.</typeparam>
/// <typeparam name="T8">The eighth parameter type.</typeparam>
public class TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> : TheoryData, IReadOnlyCollection<(T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2, T3, T4, T5, T6, T7, T8)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2, T3, T4, T5, T6, T7, T8)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	/// <param name="p3">The third data value.</param>
	/// <param name="p4">The fourth data value.</param>
	/// <param name="p5">The fifth data value.</param>
	/// <param name="p6">The sixth data value.</param>
	/// <param name="p7">The seventh data value.</param>
	/// <param name="p8">The eighth data value.</param>
	public void Add(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8) =>
		AddRow(p1, p2, p3, p4, p5, p6, p7, p8);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2, T3, T4, T5, T6, T7, T8) row) =>
		Add(row.Item1, row.Item2, row.Item3, row.Item4, row.Item5, row.Item6, row.Item7, row.Item8);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8> row) =>
		AddRow(row);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2, x.p3, x.p4, x.p5, x.p6, x.p7, x.p8 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>)this)
		{
			var data = row.GetData();
			Debug.Assert(data != null);
			yield return ((T1?)data[0], (T2?)data[1], (T3?)data[2], (T4?)data[3], (T5?)data[4], (T6?)data[5], (T7?)data[6], (T8?)data[7]);
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 9 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
/// <typeparam name="T3">The third parameter type.</typeparam>
/// <typeparam name="T4">The fourth parameter type.</typeparam>
/// <typeparam name="T5">The fifth parameter type.</typeparam>
/// <typeparam name="T6">The sixth parameter type.</typeparam>
/// <typeparam name="T7">The seventh parameter type.</typeparam>
/// <typeparam name="T8">The eighth parameter type.</typeparam>
/// <typeparam name="T9">The ninth parameter type.</typeparam>
public class TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> : TheoryData, IReadOnlyCollection<(T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2, T3, T4, T5, T6, T7, T8, T9)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8, T9}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8, T9>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	/// <param name="p3">The third data value.</param>
	/// <param name="p4">The fourth data value.</param>
	/// <param name="p5">The fifth data value.</param>
	/// <param name="p6">The sixth data value.</param>
	/// <param name="p7">The seventh data value.</param>
	/// <param name="p8">The eighth data value.</param>
	/// <param name="p9">The ninth data value.</param>
	public void Add(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9) =>
		AddRow(p1, p2, p3, p4, p5, p6, p7, p8, p9);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2, T3, T4, T5, T6, T7, T8, T9) row) =>
		Add(row.Item1, row.Item2, row.Item3, row.Item4, row.Item5, row.Item6, row.Item7, row.Item8, row.Item9);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8, T9> row) =>
		AddRow(row);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2, x.p3, x.p4, x.p5, x.p6, x.p7, x.p8, x.p9 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8, T9>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>)this)
		{
			var data = row.GetData();
			Debug.Assert(data != null);
			yield return ((T1?)data[0], (T2?)data[1], (T3?)data[2], (T4?)data[3], (T5?)data[4], (T6?)data[5], (T7?)data[6], (T8?)data[7], (T9?)data[8]);
		}
	}
}

/// <summary>
/// Represents a set of data for a theory with 10 parameters. Data can
/// be added to the data set using the collection initializer syntax.
/// </summary>
/// <typeparam name="T1">The first parameter type.</typeparam>
/// <typeparam name="T2">The second parameter type.</typeparam>
/// <typeparam name="T3">The third parameter type.</typeparam>
/// <typeparam name="T4">The fourth parameter type.</typeparam>
/// <typeparam name="T5">The fifth parameter type.</typeparam>
/// <typeparam name="T6">The sixth parameter type.</typeparam>
/// <typeparam name="T7">The seventh parameter type.</typeparam>
/// <typeparam name="T8">The eighth parameter type.</typeparam>
/// <typeparam name="T9">The ninth parameter type.</typeparam>
/// <typeparam name="T10">The tenth parameter type.</typeparam>
public class TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : TheoryData, IReadOnlyCollection<(T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?)>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> class.
	/// </summary>
	public TheoryData()
	{ }

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(IEnumerable<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> values)
	{
		Guard.ArgumentNotNull(values);

		AddRange(values.ToArray());
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params (T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)[] values) =>
		AddRange(values);

	/// <summary>
	/// Initializes a new instance of the <see cref="TheoryData{T1, T2, T3, T4, T5, T6, T7, T8, T9, T10}"/> class.
	/// </summary>
	/// <param name="values">The initial set of values</param>
	public TheoryData(params TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>[] values) =>
		AddRange(values);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="p1">The first data value.</param>
	/// <param name="p2">The second data value.</param>
	/// <param name="p3">The third data value.</param>
	/// <param name="p4">The fourth data value.</param>
	/// <param name="p5">The fifth data value.</param>
	/// <param name="p6">The sixth data value.</param>
	/// <param name="p7">The seventh data value.</param>
	/// <param name="p8">The eighth data value.</param>
	/// <param name="p9">The ninth data value.</param>
	/// <param name="p10">The tenth data value.</param>
	public void Add(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10) =>
		AddRow(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add((T1, T2, T3, T4, T5, T6, T7, T8, T9, T10) row) =>
		Add(row.Item1, row.Item2, row.Item3, row.Item4, row.Item5, row.Item6, row.Item7, row.Item8, row.Item9, row.Item10);

	/// <summary>
	/// Adds data to the theory data set.
	/// </summary>
	/// <param name="row">The data row.</param>
	public void Add(TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> row) =>
		AddRow(row);

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params (T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10)[] values)
	{
		Guard.ArgumentNotNull(values);

		AddRows(values.Select(x => new object?[] { x.p1, x.p2, x.p3, x.p4, x.p5, x.p6, x.p7, x.p8, x.p9, x.p10 }));
	}

	/// <summary>
	/// Adds multiple data items to the theory data set.
	/// </summary>
	/// <param name="values">The data values.</param>
	public void AddRange(params TheoryDataRow<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>[] values) =>
		AddRows(values);

	/// <inheritdoc />
	public new IEnumerator<(T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?)> GetEnumerator()
	{
		foreach (var row in (IEnumerable<ITheoryDataRow>)this)
		{
			var data = row.GetData();
			Debug.Assert(data != null);
			yield return ((T1?)data[0], (T2?)data[1], (T3?)data[2], (T4?)data[3], (T5?)data[4], (T6?)data[5], (T7?)data[6], (T8?)data[7], (T9?)data[8], (T10?)data[9]);
		}
	}
}
