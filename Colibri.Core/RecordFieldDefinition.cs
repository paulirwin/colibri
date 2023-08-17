﻿namespace Colibri.Core;

public record RecordFieldDefinition(RecordTypeDefinition RecordType,
    Symbol Name,
    Symbol Accessor,
    Symbol? Modifier = null);