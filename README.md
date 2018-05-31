# KTA API Samples

These are bits of code I've written to test or demonstrate aspects of the KTA API.

## Post Build Behavior

A unique aspect of this project is that the post build event uses a command line utility I wrote to update the dll directly in the assembly store (KTA database).  This makes it much quicker to cycle between testing and changing the code.