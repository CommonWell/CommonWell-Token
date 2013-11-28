=================================================
Transforming using document and segment tranforms
=================================================

The purpose of this "How To" is to show you how to create transforms at the document and segment levels. This is useful if you want to take document A and make it look like document B in terms of the segments it features and how each segment looks in terms of what fields they are made up of. So, assume that you wanted to transform the below HL7 document into another document that only featured the MSH and PID segments with a particular subset of fields.::

	MSH|^~\&|XRAY|Fel Permanente|CDB|Al Permanente|200006021411||ORU^R01|K172|P|2.6|||AL|NE
	PID|1|92380^^^XO^MRN|191919^^^GOOD HEALTH HOSPITAL^MR^GOOD HEALTH HOSPITAL^^^USSSA^SS|253763|EVERYMAN^JOE^A||19560129|M||2076-8|2222 HOME STREET^^ISHPEMING^MI^49849^""^||555-555-2004|555-555-2004||S|BAP|10199925^^^GOOD HEALTH HOSPITAL^AN|371-66-9256
	OBR|1|X89-1501^OE|78912^RD|71020^CHEST XRAY AP \T\ LATERAL|||19873290800
	OBX|1|CWE|71020&IMP^RADIOLOGIST'S IMPRESSION|4|^MASS LEFT LOWER LOBE|||A|||F
	OBX|2|CWE|71020&IMP|2|^INFILTRATE RIGHT LOWER LOBE|||A|||F
	OBX|3|CWE|71020&IMP|3|^HEART SIZE NORMAL|||N|||F|
	OBX|4|FT|71020&GDT|1|circular density (2 x 2 cm) is seen in the posterior segment of the LLL. A second, less well-defined infiltrated circulation density is seen in the R mid lung field and appears to cross the minor fissure#||||||F
	OBX|5|CWE|71020&REC|5|71020^Follow up CXR 1 month||30-45||||F


To perform the mentioned task please follow steps::


Step 1: Define how the new HL7 document will look like in terms of what segments it will consist of
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	public interface TestDocumentTransform
	{
		Segment<MSHSegment> MSH { get; }
		Segment<PIDSegment> PID { get; }
		Segment<OBRSegment> OBR { get; }
		Segment<Repeating<OBXSegment>> OBX { get; }
	}


Step 2: Define how each segment that you want to change will look like in terms of what fields it will have. Notice in step 1 that if the segment must appear in the new document verbatim then a transform should not be added (e.g. OBR segment).
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	public interface TestPIDTransform :
		HL7Transform<PIDSegment>
	{
		Value<Repeating<CX>> PatientIdList { get; }
		Value<Repeating<XPN>> PatientName { get; }
		Value<DateTime> DateOfBirth { get; }
		Value<string> Gender { get; }
		Value<Repeating<CWE>> Race { get; }
		Value<Repeating<XAD>> Address { get; }
	}

	public class TestPIDTransformMap :
		HL7TransformMap<TestPIDTransform>
	{
		public TestPIDTransformMap()
		{
			Map(x => x.PatientIdList);
			Map(x => x.PatientName);
			Map(x => x.Gender);
			Map(x => x.Race);
			Map(x => x.Address);
		}
	}

	public interface TestOBXTransform :
		HL7Transform<OBXSegment>
	{
		Value<int> SetId { get; }
		Value<string> ValueType { get; }
		Value<CWE> ObservationId { get; }
		Value<string> ObservationSubId { get; }
		Value<Repeating<string>> ObservationValue { get; }
	}

	public class TestOBXTransformMap :
		HL7TransformMap<TestOBXTransform>
	{
		public TestOBXTransformMap()
		{
			Map(x => x.SetId);
			Map(x => x.ValueType);
			Map(x => x.ObservationId);
			Map(x => x.ObservationSubId);
			Map(x => x.ObservationValue);
		}
	}

	public interface TestMSHTransform :
		HL7Transform<MSHSegment>
	{
		Value<string> EncodingCharacters { get; }
		Value<HD> SendingApplication { get; }
		Value<HD> SendingFacility { get; }
		Value<HD> ReceivingApplication { get; }
		Value<HD> ReceivingFacility { get; }
		Value<DateTime> CreationDateTime { get; }
		Value<MSG> Type { get; }
		Value<string> VersionId { get; }
	}

	public class TestMSHTransformMap :
		HL7TransformMap<TestMSHTransform>
	{
		public TestMSHTransformMap()
		{
			Map(x => x.EncodingCharacters);
			Map(x => x.SendingApplication);
			Map(x => x.SendingFacility);
			Map(x => x.ReceivingApplication);
			Map(x => x.ReceivingFacility);
			Map(x => x.Type);
			Map(x => x.VersionId);
		}
	}


Step 3: Create the document transform map based on the segment transforms created in step 2. Notice the use of the method Use. This method will transform the corresponding segment as specified by the provided transform.
""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	public class TestDocumentTransformMap :
		HL7DocumentTransformMap<TestDocumentTransform>
	{
		public TestDocumentTransformMap()
		{
			Map(x => x.MSH).Use<TestMSHTransform>();
			Map(x => x.PID).Use<TestPIDTransform>();
			Map(x => x.OBR);
			Map(x => x.OBX).Use<TestOBXTransform>();
		}
	}


Step 4: Specify the transform you want to use to transform the document by calling the TransformDocument method on the Transformer API like this,
"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	var transformedDoc = transformer.TransformDocument<TestDocumentTransform>(originalDoc);


Below is a full example of how to use the Transformer API with the intent of transforming an HL7 document to another HL7 document.

.. sourcecode:: csharp

	var originalDoc = _parser.ParseDocument(hl7);
	var transformer = HL7TransformerFactory.New(_specification);
	var formatter = new TextHL7DocumentFormatter();
	var transformedDoc = transformer.TransformDocument<TestDocumentTransform>(originalDoc);

	Console.WriteLine("Original HL7:\n{0}", originalDoc.ToString(formatter));
	Console.WriteLine("Transformed HL7:\n{0}", transformedDoc.ToString(formatter));


So, given the following HL7 document...::

	MSH|^~\&|XRAY|Fel Permanente|CDB|Al Permanente|200006021411||ORU^R01|K172|P|2.6|||AL|NE
	PID|1|92380^^^XO^MRN|191919^^^GOOD HEALTH HOSPITAL^MR^GOOD HEALTH HOSPITAL^^^USSSA^SS|253763|EVERYMAN^JOE^A||19560129|M||2076-8|2222 HOME STREET^^ISHPEMING^MI^49849^""^||555-555-2004|555-555-2004||S|BAP|10199925^^^GOOD HEALTH HOSPITAL^AN|371-66-9256
	OBR|1|X89-1501^OE|78912^RD|71020^CHEST XRAY AP \T\ LATERAL|||19873290800
	OBX|1|CWE|71020&IMP^RADIOLOGIST'S IMPRESSION|4|^MASS LEFT LOWER LOBE|||A|||F
	OBX|2|CWE|71020&IMP|2|^INFILTRATE RIGHT LOWER LOBE|||A|||F
	OBX|3|CWE|71020&IMP|3|^HEART SIZE NORMAL|||N|||F|
	OBX|4|FT|71020&GDT|1|circular density (2 x 2 cm) is seen in the posterior segment of the LLL. A second, less well-defined infiltrated circulation density is seen in the R mid lung field and appears to cross the minor fissure#||||||F
	OBX|5|CWE|71020&REC|5|71020^Follow up CXR 1 month||30-45||||F


Tranformer will transform it into the following HL7 document based on the transforms that were specified in steps 1 - 3...::

	MSH|^~\&|XRAY|Fel Permanente|CDB|Al Permanente|200006021411||ORU^R01|||2.6
	PID|||191919^^^GOOD HEALTH HOSPITAL^MR^GOOD HEALTH HOSPITAL^^^USSSA^SS||EVERYMAN^JOE^A||19560129|M||2076-8|2222 HOME STREET^^ISHPEMING^MI^49849^""^
	OBR|1|X89-1501^OE|78912^RD|71020^CHEST XRAY AP \T\ LATERAL|||19873290800
	OBX|1|CWE|71020&IMP^RADIOLOGIST'S IMPRESSION|4|^MASS LEFT LOWER LOBE
	OBX|2|CWE|71020&IMP|2|^INFILTRATE RIGHT LOWER LOBE
	OBX|3|CWE|71020&IMP|3|^HEART SIZE NORMAL
	OBX|4|FT|71020&GDT|1|circular density (2 x 2 cm) is seen in the posterior segment of the LLL. A second, less well-defined infiltrated circulation density is seen in the R mid lung field and appears to cross the minor fissure#
	OBX|5|CWE|71020&REC|5|71020^Follow up CXR 1 month


