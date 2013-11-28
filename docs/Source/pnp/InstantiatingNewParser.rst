Usage Patterns: Instantiating and initializing a new parser
===========================================================

It is recommended that you instantiate the parser in a place that is globally accessible to all parts of the application that will need to use it as is.

``
public static HL7DocumentParser Parser;

if (Parser == null)
{
	Parser = HL7ParserFactory.New(Specification);
}
``
