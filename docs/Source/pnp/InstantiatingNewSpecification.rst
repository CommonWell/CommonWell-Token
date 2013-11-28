Usage Patterns: Instantiating and initializing a new specification
==================================================================

It is recommended that you instantiate the specification in a place that is globally accessible to all parts of the application that will need to use it as is.

``
public static DocumentSpecification<HL7Document, HL7Content> Specification;

if (Specification == null)
{
	Specification = HL7SpecificationFactory.New();
}
``
