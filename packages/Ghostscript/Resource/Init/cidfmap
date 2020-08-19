%!
% This is a sample map file for CIDFont category.

%
% The map is a set of records, which must use one of the two formats :
%
% 1. A substutution of a CIDFont font with another CIDFont :
%
%    /substituted /original ;
%
%    Where 'substituted' is a name being used in a document,
%    'original' is a name of an available resource.
%
% 2. A substutution of a CIDFont font with a True Type font :
%
%    /substituted << /FileType /TrueType /Path (path) /SubfontID 1 /CSI [(ordering) supplement] >> ;
%
%    Where 'substituted' is a name being used in a document,
%    'path' is a path to a True Type font file,
%    'ordering' is a value of Ordering required for CIDSystemInfo dictionary,
%    'supplement' is a value of Supplement required for CIDSystemInfo dictionary.
%    'SubfontID' is the index of the desired font in a TTC - ignored if the target is not a TTC.
%
%    The Ordering, and Registry (if present) entries should be PostScript string types
%    ie enclosed in parentheses, not PostScript name type (initial character '/'). See
%    gs/doc/Use.htm#CIDFontSubstitution
%
%    Alternatively:
%    (substituted name) << /FileType /TrueType /Path (path) /SubfontID 1 /CSI [(ordering) supplement] >> ;
%
%    This is where the CIDFont name in the document contains a space character. Using a string instead of a name
%    allows this work - Ghostscript will internally convert the key string with a space to a name.
%
%
% Examples :
%    
%   /ShinGo-Bold   /HeiseiKakuGo-W5 ;
%   /Ryumin-Medium << /FileType /TrueType /Path (H:/AuxFiles/Fonts/BATANG.TTC) /SubfontID 3 /CSI [(Japan1) 2] >> ;
%   /Ryumin-Medium << /FileType /TrueType /Path (windir) getenv not {(c:/windows)}if (/Fonts/BATANG.TTC)concatstrings /SubfontID 3 /CSI [(Japan1) 2] >> ;
%
%   NOTE: getenv returns a boolean to indicate success of or failure, as well as the value on success. In the example, it uses this
%         to include a default value should getenv fail.
%
%  (Century Gothic) << /FileType /TrueType /Path (/usr/share/fonts/truetype/fonts-japanese-gothic.ttf) /CSI [(Identity) 1] >> ;
%
%  Note the use of a string to cope with the name containing a space.
%
