==============================================
Create a specification using extension methods
==============================================

The purpose of this "How To" is to show you how to create your own parsable specification using extension methods. So, let's assume that you wanted to create a specification so that you could parse only ADT trigger events. Schemacina makes this simple, just follow the below steps.


Step 1: Create an extension method
""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	public static class MySpecialHL7SpecificationExtensions
	{
	   public static void LoadADTSpec(this HL7SpecificationConfigurator configurator)
	   {
		  configurator.Add(typeof(MSHSegmentMap));
		  configurator.Add(typeof(PIDSegmentMap));
		  configurator.Add(typeof(PV1SegmentMap));
		  configurator.Add(typeof(SFTSegmentMap));
		  configurator.Add(typeof(UACSegmentMap));
		  configurator.Add(typeof(EVNSegmentMap));
		  configurator.Add(typeof(PV2SegmentMap));
		  configurator.Add(typeof(ARVSegmentMap));
		  configurator.Add(typeof(ROLSegmentMap));
		  configurator.Add(typeof(DB1SegmentMap));
		  configurator.Add(typeof(OBXSegmentMap));
		  configurator.Add(typeof(PDASegmentMap));
		  configurator.Add(typeof(MSASegmentMap));
		  configurator.Add(typeof(ERRSegmentMap));
		  configurator.Add(typeof(DG1SegmentMap));
		  configurator.Add(typeof(AL1SegmentMap));
		  configurator.Add(typeof(DRGSegmentMap));
		  configurator.Add(typeof(PR1SegmentMap));
		  configurator.Add(typeof(GT1SegmentMap));
		  configurator.Add(typeof(IN1SegmentMap));
		  configurator.Add(typeof(IN2SegmentMap));
		  configurator.Add(typeof(IN3SegmentMap));
		  configurator.Add(typeof(ACCSegmentMap));
		  configurator.Add(typeof(NK1SegmentMap));
		  configurator.Add(typeof(UB1SegmentMap));
		  configurator.Add(typeof(UB2SegmentMap));
		  configurator.Add(typeof(MRGSegmentMap));
	   }
	}


Step 2: Instantiate and initialize a new specification
""""""""""""""""""""""""""""""""""""""""""""""""""""""

.. sourcecode:: csharp

	var spec = HL7SpecificationFactory.New(x => { x.LoadADTSpec(); });


Please note that this will only load the segments specified in the extension method and that it is possible to define and call multiple extension methods when instantiating a specification. This means that you must also include any complex types that the corresponding segments require for definition or else Schemacina will throw an exception when attempting to load the first segment that references a type not currently in memory.

So, now you have created a specification specific to parsing ADT messages given the segments specified.
