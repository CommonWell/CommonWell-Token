============================
Create user-defined segments
============================

The purpose of this "How To" is to show you how to create a user-defined segment. In HL7 such segments are often referred to as Z segments.


Step 1: Define what the segment looks like in terms of its fields and their corresponding data types
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

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


Step 2: Define how the field mappings from the segment definition
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

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


Step 3: Load the newly created segment into a specification
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	private DocumentSpecification<HL7Document, HL7Content> _spec;

	if (_spec == null)
	{
		_spec = HL7SpecificationFactory.New(x =>
			{
				x.Add(typeof(MSHSegmentMap));
				x.Add(typeof(ZRHSegmentMap));
			});
	}



Performing the above steps will allow the parser to parse the following HL7 document::

	MSH|^~\&|MEDWAY|MEDWAY|MYapp1|Myapp2|200309121053||ADT^A24|1063360560218C000.1.|P|2.6|||AL|NE
	ZRH|000698172^^^M^D~7652981726^^^M^N|8a907fba-1d37-4978-ab16-d94172b6b1d2|20120912105510|20120915105025|ADT^A24

