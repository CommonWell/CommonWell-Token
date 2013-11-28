Getting Started with LINQ
-------------------------

To demonstrate the power of LINQ to HL7/X12, consider the following code snippet.

.. sourcecode:: csharp

    using Schemacina;
    using Schemacina.HL7;

    public bool IsDocumentAnADT(HL7Document document)
    {
        var result = document.Select<MSHSegment>()
            .Where(s => s.Type, x => x.Where(v => v.MessageCode == "ADT"))
            .Any();

        return result;
    }

The above is a simple example using the basic ``Select`` method. Select is used to match a segment type from the start of the document to the end. In this case, there should only be one ``MSHSegment``, so we are applying additional conditions (using the ``Where`` extension method) and using ``Any`` to return a ``bool`` if a match is found.

More complex queries can also be created using the ``Query`` method on the document. For example, if the OBR/OBX loops are needed from the document.

.. sourcecode:: csharp
    :linenos:

    public static ContentParser<HL7Content, QueryResult> Observation(this ContentParser<HL7Content> parser)
    {
        if (parser == null)
            throw new ArgumentNullException("parser");

        return (from msh in parser.One<MSHSegment>()
                from pid in parser.One<PIDSegment>()
                from observations in
                    (
                        from obr in parser.Any.UntilThen(parser.One<OBRSegment>())
                        from notes in parser.One<NTESegment>().ZeroOrMore()
                        from obxs in
                            (
                                from obx in parser.One<OBXSegment>()
                                from nteSegments in parser.One<NTESegment>().ZeroOrMore()
                                select new ObservationGroup(obx, nteSegments)).ZeroOrMore()
                        select new ObservationLoop(obr, notes, obxs)).OneOrMore()
                select new QueryResult(msh, pid, observations));
    }

Now that's what I'm talking about -- with one query the complexity of the OBR/OBX relationship, along with the note segments, has been expressed in a way that can be easily understood *(yes, easy is a relative term, but believe me, this is easy once you get the hang of it -- particularly compared to the way we used to do this stuff)*.

In the query contained in the extension method above, the initial MSH and PID segments are captured. After that, the query searches for an OBR segment, captures any NTE segments that follow, and then captures any OBX (and their own associated NTE segments) into a heirarchy of collections that are then returned as a single ``QueryResult`` (which is just a class created to represent the results -- since we are creating an extension method we can't pass an anonymous type across a method boundary).

Now, anytime a developer wants to pull the observations from the message, they can simply use the extension method as shown.

.. sourcecode:: csharp

    var query = document.Query(x => x.Observation());

    QueryResult result = query.First();

    string identifier = result.OBRs[0].OBR
        .Maybe(x => x.UniversalServiceIdentifier)
        .Maybe(x => x.Identifier)
        .ValueOrDefault();

That's it! The content has been extracted from the document. The first line uses the ``Query`` method and uses the ``Observation`` extension method declared above to parse the observations. Since queries return enumerations of results (we are matching patterns, and patterns can repeat), ``First`` is used to grab the first result (in this query, we are capturing the loops as part of the result). The next few lines show how a value within the OBR segment is selected and returned, with proper checking to see if the value is present in the original source document (the raw HL7 text in this case).