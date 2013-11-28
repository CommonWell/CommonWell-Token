Using Schemacina Templates
==========================

The best way to understand Schemacina templates is to see them in use. So take a look at how templates are used to map an HL7 document below.

.. sourcecode:: csharp
    :linenos:

    {
        // this initializes the template factory, once per process is all that's needed
        var templateFactory = TemplateFactory.New(x => x.LoadVersion26Templates());

        // parse the document
        var document = _parser.ParseDocument(hl7);

        // creates a populated template from the document
        var r01 = templateFactory.GetTemplate<R01Event>(document);
    }

In the snippet above, an HL7 string that contains an R01 event is parsed into a Schemacine ``HL7Document``, which is then passed to the template factory, where the developer requests the document is used to populate the requested template, which in this case is the ``R01Event`` template. There are many templates defined using the template compiler inside the ``TemplateFactory``. 

To get an idea of the template structure, the ``R01Event`` source is shown below.

.. sourcecode:: csharp
    :linenos:

    // the top level event template
    public interface R01Event :
        HL7Template
    {
        // segment properties are for single segments from the source document
        Segment<MSHSegment> MSH { get; }

        // segments can be repeating if multiple are allowed
        Segment<Repeating<SFTSegment>> SFT { get; }
        Segment<UACSegment> UAC { get; }

        // a group is a non-segment type that is another template (shown below)
        Group<Repeating<PatientResult1Group>> PatientResult { get; }

        Segment<DSCSegment> DSC { get; }
    }

    // this is the PatientResult group defined above
    public interface PatientResult1Group :
        HL7Template
    {
        Group<Patient5Group> Patient { get; }
        Group<OrderObservation1Group> OrderObservation { get; }
    }

    public interface Patient5Group :
        HL7Template
    {
        Segment<PIDSegment> PID { get; }
        Segment<PD1Segment> PD1 { get; }
        Segment<Repeating<NTESegment>> NTE { get; }
        Segment<Repeating<NK1Segment>> NK1 { get; }
        Segment<Repeating<OBXSegment>> OBX { get; }
        Group<PatientVisit1Group> PatientVisitGroup { get; }
    }

    public interface PatientVisit1Group :
        HL7Template
    {
        Segment<PV1Segment> PV1 { get; }
        Segment<PV2Segment> PV2 { get; }
    }

    public interface OrderObservation1Group :
        HL7Template
    {
        Segment<ORCSegment> ORC { get; }
        Segment<OBRSegment> OBR { get; }
        Segment<Repeating<NTESegment>> NTE { get; }
        Segment<Repeating<ROLSegment>> ROL { get; }

        // groups can also be repeating
        Group<Repeating<TimingQuantityGroup>> TimingQuantity { get; }

        Segment<CTDSegment> CTD { get; }
        Group<Repeating<ObservationDetail1Group>> Observation { get; }
        Segment<FT1Segment> FT1 { get; }
        Segment<CTISegment> CTI { get; }
        Group<Repeating<SpecimenGroup>> Specimen { get; }
    }

    public interface TimingQuantityGroup :
        HL7Template
    {
        Segment<TQ1Segment> TQ1 { get; }
        Segment<Repeating<TQ2Segment>> TQ2 { get; }
    }

    public interface ObservationDetail1Group :
        HL7Template
    {
        Segment<OBXSegment> OBX { get; }
        Segment<Repeating<NTESegment>> NTE { get; }
    }

    public interface SpecimenGroup :
        HL7Template
    {
        Segment<SPMSegment> SPM { get; }
        Segment<Repeating<OBXSegment>> OBX { get; }
    }

The templates are all defined using interfaces, and define the contract for the expected layout. For each template defined, a corresponding ``TemplateMap`` must also be created. The maps for the above templates are shown below.

.. sourcecode:: csharp
    :linenos:

    public class R01EventMap :
        HL7TemplateMap<R01Event>
    {
        public R01EventMap()
        {
            // segments are optional by default, .Required() makes it mandatory
            Map(x => x.MSH, 1).Required();
            Map(x => x.SFT, 2);
            Map(x => x.UAC, 3);
            Map(x => x.PatientResult, 4);
            Map(x => x.DSC, 5);
        }
    }

    // this map is referenced by the .PatientResult map
    public class PatientResult1GroupMap :
        HL7TemplateMap<PatientResult1Group>
    {
        public PatientResult1GroupMap()
        {
            Map(x => x.Patient).Required();
            Map(x => x.OrderObservation);
        }
    }

    // the rest are cut out, but can be found in the source

Each template map defines the properties that are mapped in the order they should be mapped. Maps can also include conditional statements to ensure the source document meets the requirements of the template, such as checking the ``MessageType`` in the document as shown below.

.. sourcecode:: csharp

    Map(x => x.MSH, 1)
        .Where(x => x.Maybe(v => v.MessageType).Maybe(v => v.TriggerEvent).EqualTo("R01"))
        .Required();

This allow the template to define constraints on the source document to prevent accidental mapping to unexpected types.

So that's an introduction, there are many events already defined in the specifications. Templates are faster than the LINQ support, and save the trouble of defining the document structure. 

However, if a document that doesn't conform to an existing template is desired, the developer can always create a new template to handle it and add it to the Template Factory. The template compiler emits useful errors and warnings if the template cannot be compiled to help ensure the template is correctly defined (but doesn't check that it actually meets the industry standard specification).
