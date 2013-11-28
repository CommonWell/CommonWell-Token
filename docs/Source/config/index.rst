Schemacina Configuration
========================

Schemacina used an internal Domain Specific Language (DSL) for configuration.

Getting Started
---------------

Once the Schemacina assemblies have been added to your project (see the Install section), a parser must be configured. First, however, a parser requires a specification for the structured document format and mapping. Using HL7 as an example, the method below creates both a specfication and parser.

.. sourcecode:: csharp

    using Schemacina;
    using Schemacina.HL7;

    public HL7DocumentParser CreateHL7Parser()
    {
        var specification = HL7SpecificationFactory.New();
        var parser = HL7ParserFactory.New(specification);

        return parser;
    }

To create a specification, the ``HL7SpecificationFactory`` is used. In this case, the default configuration loads all the HL7 segments and components contained in the Schemacina.HL7 assembly. If only a subset of the segments or components are needed, they can be specified individually.

Once the specification has been created, the HL7ParserFactory is used to create the parser. The parser requires a specification, which has been supplied as shown. 

    *Specifications and parsers are* **expensive** *to create. Schemacina is* **fully thread-safe** *so that a single specification and parser instance can be shared by the entire process. This keeps performance high and memory usage low. It's recommended that the parser be created at application startup and referenced as a singleton (using either a static or a singleton lifecycle in a container).* 

Now that we have a parser, we can start parsing documents.

.. sourcecode:: csharp

    public void DoSomethingWithDocument(string hl7text)
    {
        HL7Document document = _parser.ParseDocument(hl7text);

        // start doing things with the document
    }
    
The ``ParseDocument`` method does a quick scan of the document and returns immediately. Schemacina is a lazy parser, so the actual contents are not parsed until they are accessed by the application. Once parsed, the parsed data is cached in the document so that subsequent accesses are immediate. The parsing and caching logic within the document are completely thread-safe as the document contents are immutable *(yes, a lot of functional programming techniques being used inside)*.

