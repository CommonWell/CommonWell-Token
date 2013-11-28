====================================================
Parse a document based on a particular specification
====================================================

The purpose of this "How To" is to show you how to parse a document based on a particular specification. So, assume that you wanted to load a subset of the HL7 specification you would follow the below steps.


Step 1: Instantiate and initialize a new specification
""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	var spec = HL7SpecificationFactory.New(x => { x.LoadVersion26HL7(); });


Step 2: Instantiate and initialize a new parser
"""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	var parser = HL7ParserFactory.New(spec);


Remember, instantiating both a new specification and parser are very expensive and should be taken into consideration in your specific implementation. It is suggested that you either define static variables that are publicly accessible, use a getter property, or implement the Singleton pattern.


#################
Singleton Pattern
#################

.. sourcecode:: csharp

	public sealed class MySchemacinaSingletonClass
	{
		private static DocumentSpecification<HL7Document, HL7Content> _specification;
		private static HL7DocumentParser _parser;

		public static DocumentSpecification<HL7Document, HL7Content> GetSpecification()
		{
			if (_specification == null)
			{
				_specification = HL7SpecificationFactory.New(x => { x.LoadVersion26HL7(); });
			}

			return _specification;
		}
		
		public static HL7DocumentParser GetParser()
		{
			if (_parser == null)
			{
				_parser = HL7ParserFactory.New(GetSpecification())
			}
		}
	}


#########################################
Singleton Pattern (multithreaded version)
#########################################

.. sourcecode:: csharp

	public sealed class MySchemacinaMultithreadedSingletonClass
	{
		private static volatile DocumentSpecification<HL7Document, HL7Content> _specification;
		private static object _obj = new Object();

		public static DocumentSpecification<HL7Document, HL7Content> GetSpecification()
		{
			if (_specification == null)
			{
				lock (_obj)
				{
					if (_specification == null)
					{
						_specification = HL7SpecificationFactory.New(x => { x.LoadVersion26HL7(); });
					}
				}
			}

			return _specification;
		}
	}


###############
Getter Property
###############

.. sourcecode:: csharp

	public sealed class MySchemacinaClass
	{
		private DocumentSpecification<HL7Document, HL7Content> _specification;

		public DocumentSpecification<HL7Document, HL7Content> Specification
		{
			get
			{
				if (_specification == null)
				{
					_specification = HL7SpecificationFactory.New(x => { x.LoadVersion26HL7(); });
				}

				return _specification;
			}
		}
	}
