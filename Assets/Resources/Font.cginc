RWStructuredBuffer<Line> _fontRW;

#define LINES_PER_CHARACTER 16

void AddFontLine(int idx, float2 a, float2 b) {
    Line l;
    l.a = a;
    l.b = b;
    _fontRW[idx] = l;
}


void BuildCharacter(int character) {
    int idx = character * LINES_PER_CHARACTER;

    //first wipe it for good form
    for (int i = 0; i < LINES_PER_CHARACTER; i++) {
        Line blank;
        blank.a = blank.b = 0;
        _fontRW[idx + i] = blank;
    }

    switch (character) {
    case 'a': 
    case 'A':
    {
        AddFontLine(idx++, float2(0, 0), float2(0, 0.5f));
        AddFontLine(idx++, float2(1, 0), float2(1, 0.5f));
        AddFontLine(idx++, float2(0.5f, 1), float2(0, 0.5f));
        AddFontLine(idx++, float2(0.5f, 1), float2(1, 0.5f));
        AddFontLine(idx++, float2(0, 0.5f), float2(1, 0.5f));

        break;
    }
    case 'b':
    case 'B':
    {
        float2 bl = float2(0, 0);
        float2 ml = float2(0, 0.5f);
        float2 tl = float2(0, 1);
        
        float2 corner0 = float2(0.9f, 0);
        float2 corner1 = float2(1.0f, 0.1f);
        float2 corner2 = float2(1.0f, 0.4f);
        float2 corner3 = float2(0.9f, 0.5f);
        float2 corner4 = float2(1.0f, 0.6f);
        float2 corner5 = float2(1.0f, 0.9f);
        float2 corner6 = float2(0.9f, 1.0f);

        AddFontLine(idx++, bl, corner0);
        AddFontLine(idx++, ml, corner3);
        AddFontLine(idx++, tl, corner6);
        AddFontLine(idx++, bl, tl);
        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);

        break;
    }
    case 'c':
    case 'C':
    {
        float2 corner0 = float2(1.0f, 0.2f);
        float2 corner1 = float2(0.8f, 0.0f);
        float2 corner2 = float2(0.2f, 0.0f);
        float2 corner3 = float2(0.0f, 0.2f);
        float2 corner4 = float2(0.0f, 0.8f);
        float2 corner5 = float2(0.2f, 1.0f);
        float2 corner6 = float2(0.8f, 1.0f);
        float2 corner7 = float2(1.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);

        break;
    }
    case 'd':
    case 'D':
    {
        float2 corner0 = float2(1.0f, 0.2f);
        float2 corner1 = float2(0.8f, 0.0f);
        float2 corner2 = float2(0.0f, 0.0f);
        float2 corner3 = float2(0.0f, 1.0f);
        float2 corner4 = float2(0.8f, 1.0f);
        float2 corner5 = float2(1.0f, 0.8f);
        float2 corner6 = float2(1.0f, 0.2f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);

        break;
    }
    case 'e':
    case 'E':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(1.0f, 0.0f);
        float2 corner2 = float2(0.0f, 0.5f);
        float2 corner3 = float2(1.0f, 0.5f);
        float2 corner4 = float2(0.0f, 1.0f);
        float2 corner5 = float2(1.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner0, corner4);

        break;
    }
    case 'f':
    case 'F':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(1.0f, 0.0f);
        float2 corner2 = float2(0.0f, 0.5f);
        float2 corner3 = float2(1.0f, 0.5f);
        float2 corner4 = float2(0.0f, 1.0f);
        float2 corner5 = float2(1.0f, 1.0f);

        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner0, corner4);

        break;
    }
    case 'g':
    case 'G':
    {
        float2 corner0 = float2(1.0f, 0.2f);
        float2 corner1 = float2(0.8f, 0.0f);
        float2 corner2 = float2(0.2f, 0.0f);
        float2 corner3 = float2(0.0f, 0.2f);
        float2 corner4 = float2(0.0f, 0.8f);
        float2 corner5 = float2(0.2f, 1.0f);
        float2 corner6 = float2(0.8f, 1.0f);
        float2 corner7 = float2(1.0f, 0.8f);

        float2 corner8 = float2(1.0f, 0.4f);
        float2 corner9 = float2(0.5f, 0.4f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);

        AddFontLine(idx++, corner0, corner8);
        AddFontLine(idx++, corner8, corner9);

        break;
    }
    case 'h':
    case 'H':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(0.0f, 1.0f);
        float2 corner2 = float2(1.0f, 0.0f);
        float2 corner3 = float2(1.0f, 1.0f);
        float2 corner4 = float2(0.0f, 0.5f);
        float2 corner5 = float2(1.0f, 0.5f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner4, corner5);

        break;
    }
    case 'i':
    case 'I':
    {
        float2 corner0 = float2(0.5f, 0.0f);
        float2 corner1 = float2(0.5f, 1.0f);
        float2 corner2 = float2(0.0f, 0.0f);
        float2 corner3 = float2(1.0f, 0.0f);
        float2 corner4 = float2(0.0f, 1.0f);
        float2 corner5 = float2(1.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner4, corner5);

        break;
    }
    case 'j':
    case 'J':
    {
        float2 corner0 = float2(0.0f, 0.2f);
        float2 corner1 = float2(0.2f, 0.0f);
        float2 corner2 = float2(0.8f, 0.0f);
        float2 corner3 = float2(1.0f, 0.2f);
        float2 corner4 = float2(1.0f, 1.0f);
        float2 corner5 = float2(0.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);

        break;
    }
    case 'k':
    case 'K':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(0.0f, 1.0f);
        float2 corner2 = float2(0.0f, 0.5f);
        float2 corner3 = float2(1.0f, 0.0f);
        float2 corner4 = float2(0.0f, 0.5f);
        float2 corner5 = float2(1.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner4, corner5);

        break;
    }
    case 'l':
    case 'L':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(0.0f, 1.0f);
        float2 corner2 = float2(0.0f, 0.0f);
        float2 corner3 = float2(1.0f, 0.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);

        break;
    }
    case 'm':
    case 'M':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(0.2f, 1.0f);
        float2 corner2 = float2(0.5f, 0.5f);
        float2 corner3 = float2(0.8f, 1.0f);
        float2 corner4 = float2(1.0f, 0.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);

        break;
    }
    case 'n':
    case 'N':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(0.0f, 1.0f);
        float2 corner2 = float2(1.0f, 0.0f);
        float2 corner3 = float2(1.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);

        break;
    }
    case 'o':
    case 'O':
    {
        float2 corner0 = float2(1.0f, 0.2f);
        float2 corner1 = float2(0.8f, 0.0f);
        float2 corner2 = float2(0.2f, 0.0f);
        float2 corner3 = float2(0.0f, 0.2f);
        float2 corner4 = float2(0.0f, 0.8f);
        float2 corner5 = float2(0.2f, 1.0f);
        float2 corner6 = float2(0.8f, 1.0f);
        float2 corner7 = float2(1.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);
        AddFontLine(idx++, corner7, corner0);

        break;
    }
    case 'p':
    case 'P':
    {
        float2 bl = float2(0, 0);
        float2 ml = float2(0, 0.5f);
        float2 tl = float2(0, 1);

        float2 corner2 = float2(1.0f, 0.4f);
        float2 corner3 = float2(0.9f, 0.5f);
        float2 corner4 = float2(1.0f, 0.6f);
        float2 corner5 = float2(1.0f, 0.9f);
        float2 corner6 = float2(0.9f, 1.0f);

        AddFontLine(idx++, ml, corner3);
        AddFontLine(idx++, tl, corner6);
        AddFontLine(idx++, bl, tl);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);

        break;
    }
    case 'q':
    case 'Q':
    {
        float2 corner0 = float2(1.0f, 0.2f);
        float2 corner1 = float2(0.8f, 0.0f);
        float2 corner2 = float2(0.2f, 0.0f);
        float2 corner3 = float2(0.0f, 0.2f);
        float2 corner4 = float2(0.0f, 0.8f);
        float2 corner5 = float2(0.2f, 1.0f);
        float2 corner6 = float2(0.8f, 1.0f);
        float2 corner7 = float2(1.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);
        AddFontLine(idx++, corner7, corner0);
        AddFontLine(idx++, float2(0.7f, 0.3f), float2(1.0f,0.0f));

        break;
    }
    case 'r':
    case 'R':
    {
        float2 bl = float2(0, 0);
        float2 ml = float2(0, 0.5f);
        float2 tl = float2(0, 1);

        float2 corner2 = float2(1.0f, 0.4f);
        float2 corner3 = float2(0.9f, 0.5f);
        float2 corner4 = float2(1.0f, 0.6f);
        float2 corner5 = float2(1.0f, 0.9f);
        float2 corner6 = float2(0.9f, 1.0f);
        float2 corner7 = float2(1.0f, 0.0f);

        AddFontLine(idx++, ml, corner7);
        AddFontLine(idx++, ml, corner3);
        AddFontLine(idx++, tl, corner6);
        AddFontLine(idx++, bl, tl);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);

        break;
    }

    case 's':
    case 'S':
    {
        float2 corner0 = float2(0.0f, 0.2f);
        float2 corner1 = float2(0.2f, 0.0f);
        float2 corner2 = float2(0.8f, 0.0f);
        float2 corner3 = float2(1.0f, 0.2f);
        float2 corner4 = float2(1.0f, 0.3f);
        float2 corner5 = float2(0.8f, 0.5f);
        float2 corner6 = float2(0.2f, 0.5f);
        float2 corner7 = float2(0.0f, 0.7f);
        float2 corner8 = float2(0.0f, 0.8f);
        float2 corner9 = float2(0.2f, 1.0f);
        float2 corner10 = float2(0.8f, 1.0f);
        float2 corner11 = float2(1.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);
        AddFontLine(idx++, corner7, corner8);
        AddFontLine(idx++, corner8, corner9);
        AddFontLine(idx++, corner9, corner10);
        AddFontLine(idx++, corner10, corner11);

        break;
    }
    case 't':
    case 'T':
    {
        float2 corner0 = float2(0.5f, 0.0f);
        float2 corner1 = float2(0.5f, 1.0f);
        float2 corner2 = float2(0.0f, 1.0f);
        float2 corner3 = float2(1.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);

        break;
    }
    case 'u':
    case 'U':
    {
        float2 corner0 = float2(1.0f, 0.2f);
        float2 corner1 = float2(0.8f, 0.0f);
        float2 corner2 = float2(0.2f, 0.0f);
        float2 corner3 = float2(0.0f, 0.2f);
        float2 corner4 = float2(0.0f, 0.8f);
        float2 corner5 = float2(0.0f, 1.0f);
        float2 corner6 = float2(1.0f, 1.0f);
        float2 corner7 = float2(1.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner6, corner7);
        AddFontLine(idx++, corner7, corner0);

        break;
    }
    case 'v':
    case 'V':
    {
        float2 corner0 = float2(0.0f, 1.0f);
        float2 corner1 = float2(0.5f, 0.0f);
        float2 corner2 = float2(1.0f, 1.0f);
        float2 corner3 = float2(0.5f, 0.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);

        break;
    }
    case 'w':
    case 'W':
    {
        float2 corner0 = float2(0.0f, 1.0f);
        float2 corner1 = float2(0.2f, 0.0f);
        float2 corner2 = float2(0.5f, 0.5f);
        float2 corner3 = float2(0.8f, 0.0f);
        float2 corner4 = float2(1.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);

        break;
    }
    case 'x':
    case 'X':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(1.0f, 1.0f);
        float2 corner2 = float2(1.0f, 0.0f);
        float2 corner3 = float2(0.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);

        break;
    }
    case 'y':
    case 'Y':
    {
        float2 corner0 = float2(0.0f, 1.0f);
        float2 corner1 = float2(0.5f, 0.5f);
        float2 corner2 = float2(1.0f, 1.0f);
        float2 corner3 = float2(0.5f, 0.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner1);
        AddFontLine(idx++, corner1, corner3);

        break;
    }
    case 'z':
    case 'Z':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(1.0f, 0.0f);
        float2 corner2 = float2(0.0f, 1.0f);
        float2 corner3 = float2(1.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner0, corner3);

        break;
    }
    case '0':
    {
        float2 corner0 = float2(1.0f, 0.2f);
        float2 corner1 = float2(0.8f, 0.0f);
        float2 corner2 = float2(0.2f, 0.0f);
        float2 corner3 = float2(0.0f, 0.2f);
        float2 corner4 = float2(0.0f, 0.8f);
        float2 corner5 = float2(0.2f, 1.0f);
        float2 corner6 = float2(0.8f, 1.0f);
        float2 corner7 = float2(1.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);
        AddFontLine(idx++, corner7, corner0);
        AddFontLine(idx++, float2(0.0f, 0.3f), float2(1.0f, 0.7f));

        break;
    }
    case '1':
    {
        float2 corner0 = float2(0.5f, 0.0f);
        float2 corner1 = float2(0.5f, 1.0f);
        float2 corner2 = float2(0.0f, 0.0f);
        float2 corner3 = float2(1.0f, 0.0f);
        float2 corner4 = float2(0.5f, 1.0f);
        float2 corner5 = float2(0.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner4, corner5);

        break;
    }
    case '2':
    {
        float2 corner0 = float2(1.0f, 0.0f);
        float2 corner1 = float2(0.0f, 0.0f);
        float2 corner2 = float2(0.6f, 0.4f);
        float2 corner3 = float2(1.0f, 0.6f);
        float2 corner4 = float2(1.0f, 0.9f);
        float2 corner5 = float2(0.8f, 1.0f);
        float2 corner6 = float2(0.2f, 1.0f);
        float2 corner7 = float2(0.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);

        break;
    }
    case '3':
    {
        float2 bl = float2(0, 0);
        float2 ml = float2(0, 0.5f);
        float2 tl = float2(0, 1);

        float2 corner0 = float2(0.9f, 0);
        float2 corner1 = float2(1.0f, 0.1f);
        float2 corner2 = float2(1.0f, 0.4f);
        float2 corner3 = float2(0.9f, 0.5f);
        float2 corner4 = float2(1.0f, 0.6f);
        float2 corner5 = float2(1.0f, 0.9f);
        float2 corner6 = float2(0.9f, 1.0f);

        AddFontLine(idx++, bl, corner0);
        AddFontLine(idx++, ml, corner3);
        AddFontLine(idx++, tl, corner6);
        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);

        break;
    }
    case '4':
    {
        float2 corner0 = float2(0.7f, 0.0f);
        float2 corner1 = float2(0.7f, 1.0f);
        float2 corner2 = float2(0.0f, 0.4f);
        float2 corner3 = float2(1.0f, 0.4f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);

        break;
    }
    case '5':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(0.2f, 0.0f);
        float2 corner2 = float2(0.8f, 0.0f);
        float2 corner3 = float2(1.0f, 0.2f);
        float2 corner4 = float2(1.0f, 0.3f);
        float2 corner5 = float2(0.8f, 0.5f);
        float2 corner6 = float2(0.0f, 0.5f);
        float2 corner7 = float2(0.0f, 0.7f);
        float2 corner8 = float2(0.0f, 1.0f);
        float2 corner9 = float2(0.2f, 1.0f);
        float2 corner10 = float2(1.0f, 1.0f);
        float2 corner11 = float2(1.0f, 0.8f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);
        AddFontLine(idx++, corner7, corner8);
        AddFontLine(idx++, corner8, corner9);
        AddFontLine(idx++, corner9, corner10);

        break;
    }
    case '6':
    {
        float2 corner0 = float2(0.0f, 0.2f);
        float2 corner1 = float2(0.2f, 0.0f);
        float2 corner2 = float2(0.8f, 0.0f);
        float2 corner3 = float2(1.0f, 0.2f);
        float2 corner4 = float2(1.0f, 0.3f);
        float2 corner5 = float2(0.8f, 0.5f);
        float2 corner6 = float2(0.0f, 0.5f);
        float2 corner7 = float2(0.0f, 0.7f);
        float2 corner8 = float2(0.0f, 0.8f);
        float2 corner9 = float2(0.2f, 1.0f);
        float2 corner10 = float2(0.9f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);
        AddFontLine(idx++, corner7, corner8);
        AddFontLine(idx++, corner8, corner9);
        AddFontLine(idx++, corner9, corner10);
        AddFontLine(idx++, corner0, corner7);

        break;
    }
    case '7':
    {
        float2 corner0 = float2(0.4f, 0.0f);
        float2 corner1 = float2(1.0f, 1.0f);
        float2 corner2 = float2(0.0f, 1.0f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);

        break;
    }
    case '8':
    {
        float2 corner0 = float2(0.9f, 0);
        float2 corner1 = float2(1.0f, 0.1f);
        float2 corner2 = float2(1.0f, 0.4f);
        float2 corner3 = float2(0.9f, 0.5f);
        float2 corner4 = float2(1.0f, 0.6f);
        float2 corner5 = float2(1.0f, 0.9f);
        float2 corner6 = float2(0.9f, 1.0f);

        float2 cornerl0 = float2(0.1f, 0);
        float2 cornerl1 = float2(0.0f, 0.1f);
        float2 cornerl2 = float2(0.0f, 0.4f);
        float2 cornerl3 = float2(0.1f, 0.5f);
        float2 cornerl4 = float2(0.0f, 0.6f);
        float2 cornerl5 = float2(0.0f, 0.9f);
        float2 cornerl6 = float2(0.1f, 1.0f);


        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);

        AddFontLine(idx++, cornerl0, cornerl1);
        AddFontLine(idx++, cornerl1, cornerl2);
        AddFontLine(idx++, cornerl2, cornerl3);
        AddFontLine(idx++, cornerl3, cornerl4);
        AddFontLine(idx++, cornerl4, cornerl5);
        AddFontLine(idx++, cornerl5, cornerl6);

        AddFontLine(idx++, corner0, cornerl0);
        AddFontLine(idx++, corner6, cornerl6);
        AddFontLine(idx++, corner3, cornerl3);

        break;
    }
    case '9':
    {
        float2 corner0 = float2(0.3f, 0.0f);
        float2 corner1 = float2(0.7f, 0.0f);
        float2 corner2 = float2(1.0f, 0.2f);
        float2 corner3 = float2(1.0f, 0.8f);
        float2 corner4 = float2(0.8f, 1.0f);
        float2 corner5 = float2(0.2f, 1.0f);
        float2 corner6 = float2(0.0f, 0.8f);
        float2 corner7 = float2(0.1f, 0.5f);
        float2 corner8 = float2(0.2f, 0.5f);
        float2 corner9 = float2(1.0f, 0.5f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner4);
        AddFontLine(idx++, corner4, corner5);
        AddFontLine(idx++, corner5, corner6);
        AddFontLine(idx++, corner6, corner7);
        AddFontLine(idx++, corner7, corner8);
        AddFontLine(idx++, corner8, corner9);

        break;
    }
    case ' ':
    {
        break;
    }
    case '.':
    {
        float2 corner0 = float2(0.45f, 0.0f);
        float2 corner1 = float2(0.55f, 0.0f);
        float2 corner2 = float2(0.55f, 0.1f);
        float2 corner3 = float2(0.25f, 0.1f);

        AddFontLine(idx++, corner0, corner1);
        AddFontLine(idx++, corner1, corner2);
        AddFontLine(idx++, corner2, corner3);
        AddFontLine(idx++, corner3, corner0);

        break;
    }
    case '_':
    {
        float2 corner0 = float2(0.0f, 0.0f);
        float2 corner1 = float2(1.0f, 0.0f);

        AddFontLine(idx++, corner0, corner1);

        break;
    }

    default:
    {
        //now a square
        AddFontLine(idx++, float2(0, 0), float2(0, 1));
        AddFontLine(idx++, float2(0, 1), float2(1, 1));
        AddFontLine(idx++, float2(1, 1), float2(1, 0));
        AddFontLine(idx++, float2(1, 0), float2(0, 0));
        break;
    }
    }
}
 