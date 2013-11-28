Schemacina Templates
====================

The ordered layout of structured documents supported by Schemacina, such as HL7 and X12, is actually a complex series of nested groups. These groups are of varying depths, and are difficult to parse using linear algorithms. For querying this complex document structure, the Schemacina LINQ support (via ``Query``) provides a powerful mechanism to break up the document into groups.

However, there are many predefined layouts of document data for a given document format. For example, HL7 defines a set of events, each of which has a specific layout defined in the specification. To enable the definition of events in Schemacina, a template factory is available to define and project documents into these layouts.


.. toctree::

    ShowMeSomeCode.rst

    
