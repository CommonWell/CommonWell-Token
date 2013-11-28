============================
Getting segment field values
============================

The nesting of HL7 as you decend into segments, fields, components, subcomponents, etc. can result in some
complex access logic. For example, consider the following HL7::


	MSH|^~\&|XRAY||CDB||200006021411||ORU^R01|K172|P|
	PID|||191919^^^GOOD HEALTH HOSPITAL^MR^GOOD HEALTH HOSPITAL^^^USSSA^SS|253763|EVERYMAN^JOE^A||19560129|M|||2222 HOME STREET^^ISHPEMING^MI^49849^""^||555-555-2004|555-555-2004||S|BAP|10199925^^^GOOD HEALTH HOSPITAL^AN|371-66-9256||
	OBR|1|X89-1501^OE|78912^RD|71020^CHEST XRAY AP \T\ LATERAL|||19873290800||||
	OBX|1|CWE|71020&IMP^RADIOLOGIST'S IMPRESSION|4|^MASS LEFT LOWER LOBE|||A|||F|
	OBX|2|CWE|71020&IMP|2|^INFILTRATE RIGHT LOWER LOBE|||A|||F|
	OBX|3|CWE|71020&IMP|3|^HEART SIZE NORMAL|||N|||F|
	OBX|4|FT|71020&GDT|1|circular density (2 x 2 cm) is seen in the posterior segment of the LLL. A second, less well-defined infiltrated circulation density is seen in the R mid lung field and appears to cross the minor fissure#||||||F|
	OBX|5|CWE|71020&REC|5|71020^Follow up CXR 1 month||30-45||||F|

For the purpose of this example let us assume that you needed to get the patient's religion from the above HL7 document's PID segment.

Step 1: Parse the HL7 Document
""""""""""""""""""""""""""""""

The document is parsed using the ``ParseDocument`` method on the parser.

.. sourcecode:: csharp

	var doc = _parser.ParseDocument(hl7);

Step 2: Select the Segment
""""""""""""""""""""""""""

The segment is selected using the ``Select`` method on the document.

.. sourcecode:: csharp

	var segment = doc.Select<PIDSegment>().FirstOrDefault();


Step 3: Select the Field Values
"""""""""""""""""""""""""""""""

Repeatedly select components and fields using the ``Maybe`` method on the segment. Once the final field has been selected, use the ``ValueOrDefault`` method to return the value or a default string if the value is not present in the source document.

.. sourcecode:: csharp

	string religion = segment
		.Maybe(x => x.Religion)
		.Maybe(x => x.Identifier)
		.ValueOrDefault(() => string.Empty);

The output of executing step 3 would be BAP. The extension method ValueOrDefault ensures that the developer will be returned a value even if the source text is not of the same type of what is defined in the specification for that particular field. For example, if the field data type was a DateTime and the source text was "Something Here" then accessing this field via the lazy typing mechanism Schemacina provides via the Value property would throw an exception as would attempt to type a string value as a DateTime value. The extension method ValueOrDefault has it's parallel to FirstOrDefault in that if it cannot get a proper value it returns a default with one exception. Unlike with using FirstOrDefault, ValueOrDefault allows the developer to define the default behavior if the original text cannot be typed correctly.

The alternative approach would be to to write the following code:

.. sourcecode:: csharp

	string religion = string.Empty;
	if ((segment.Religion != null) && segment.Religion.IsPresent && segment.Religion.HasValue)
	{
		if ((segment.Religion.Value.Identifier != null) && segment.Religion.Value.Identifier.IsPresent && segment.Religion.Value.Identifier.HasValue)
		{
			religion = segment.Religion.Value.Identifier.Value;
		}
	}

As you can see that chaining extension methods Maybe and ValueOrDefault together is a more fluent developer friendly approach and therefore is preferred.
