========================================
Create a specification from segment maps
========================================

The purpose of this "How To" is to show you how to parse a document based on a particular specification. So, assuming that you wanted to load a particular subset of your specification of choice you would simply do the following:


Step 1: Create a user-defined (i.e. Z segment) segment
""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	public interface ZRHSegment :
	   HL7Segment
	{
	   Value<Repeating<CX>> PatientId { get; }
	   Value<Guid> PipeId { get; }
	   Value<DateTime> CreationDateTime { get; }
	   Value<DateTime> SentDateTime { get; }
	   Value<MSG> Type { get; }
	}

	public class ZRHSegmentMap :
	   HL7SegmentMap<ZRHSegment>
	{
	   public ZRHSegmentMap()
	   {
		  Key(0, "ZRH");

		  Name("Test");

		  Map(x => x.PatientId, 1).MaxLength(250).Required();
		  Map(x => x.PipeId, 2).MaxLength(50).Required();
		  Map(x => x.CreationDateTime).LongDateTime(3).Format(HL7DateTimeFormat.HL7VariableLongDateTime).Required();
		  Map(x => x.SentDateTime).LongDateTime(4).Format(HL7DateTimeFormat.HL7VariableLongDateTime).Required();
		  Map(x => x.Type, 5).MaxLength(15).Required();
	   }
	}


Step 2: Instantiate a new specification with the segments you want the parser to recognize during parsing
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	private DocumentSpecification<HL7Document, HL7Content> _spec;

	if (_spec == null)
	{
		_spec = HL7SpecificationFactory.New(x =>
			{
				x.Add(typeof(MSHSegmentMap));
				x.Add(typeof(PIDSegmentMap));
				x.Add(typeof(PD1SegmentMap));
				x.Add(typeof(ZRHSegmentMap));
			});
	}


Step 3: Instantiate a new parser based on the specification defined in step 2
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	private HL7DocumentParser _parser;

	if (_parser == null)
	{
		_parser = HL7ParserFactory.New(_spec);
	}


