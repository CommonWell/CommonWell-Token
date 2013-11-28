LINQ Reference
==============

LINQ is a language feature of C#, and is powered by extension methods (another language feature introduced in C# 3.0). The query parser in Schemacina uses the same technique to parse segments.

    The segments of a document are the input stream for a LINQ query. As segments are *matched*, the cursor of the input stream advances. It's important to understand this concept when writing queries using the methods below.

    The input cursor is per-query, and multiple queries can be executed against the same document concurrently, keeping the parser thread-safe.


Method Reference
----------------

The following methods are available to match segments using the ``Query`` method.

Matching Methods
^^^^^^^^^^^^^^^^

.Any()
    Matches any segment in the input stream. This is typically the base from which other methods will be chained.

.One<TSegment>()
    Matches a single segment in the input stream, which must be of the specific type. If the segment at the cursor is *not* the specified type, matching stops and no results are returned.

.One<TSegment>(Func<TSegment, bool> predicate)
    Matches a single segment in the input stream, which must be of the specified type and return true for the specified predicate. If the segment at the cursor is *not* the specified type or the predicate returns false, matching stops and no results are returned.

.One(Type segmentType)
    A non-generic version of the ``.One<T>()`` method, allowing any type to be specified at runtime. With this method, the generic content type is returned. If the segment at the cursor is *not* the specified type, matching stops and no results are returned.

.One(Func<TContent, bool> predicate)
    Matches a single segment in the input stream if the predicate returns true, otherwise matching stops and no results are returned.


Collection Methods
^^^^^^^^^^^^^^^^^^

These methods can be chained to the end of matching methods to configure the frequency of the match, and typically result in a collection of segments.

.ZeroOrMore()
    Returns a collection of zero or more items. If one or more segments are matched in the input stream, a collection of those items is returned. Otherwise, an empty collection is returned. Matching continues even if no items are present.

.OneOrMore()
    Returns a collection of one or more segments. If no segments are matched, matching stops and no results are returned.

.Once()
    Returns a collection of one segment if the segment is matched. Useful for cases where a collection may be needed to avoid having to write multiple queries for single or multiple items.


Navigation Methods
^^^^^^^^^^^^^^^^^^

These methods are used to enumerate the document in various ways, and not explicitly select content.

.Until(...)
    Selects segments until the parameter query is matched and returns a collection of those elements. For example, ``x.Until(x.One<PIDSegment>())`` would skip over any segments until the next ``PIDSegment`` is found, and return the skipped segments in the collection.

.Except(...)
    Select any element except for a segment matched by the parameter query. If the segment matches the query passed, nothing is matched.

.Then(...)
    Provides a convienient way to chain navigation methods to match a simple pattern.

.UntilThen(...)
    A convenience method to skip over segments until the matching one is found and return that matching segment. For example, ``x.UntilThen(x.One<PIDSegment>())`` would skip over any segments until the next ``PIDSegment`` is found, and discard the collection of unmatched segments prior to the ``PIDSegment``.

.Range(start, end)
    An advanced method that returns a range of segments between the start and the finish used to break up a larger document into sections. Those section can then be further queried independent of the original document. This is commonly used when multiple transactions are in a single document. *A separate page should be made to demonstrate this usage.*