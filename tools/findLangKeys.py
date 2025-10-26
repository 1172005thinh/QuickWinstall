#!/usr/bin/env python3
"""
findLangKeys.py - Extract localization keys from source code

This script scans source code files for localization keys used in
lang.GetString("key") calls and outputs them to a text file.

Requirements:
- Python 3.x
- No additional libraries required

Usage:
    python findLangKeys.py                           # Scan default source directory
    python findLangKeys.py -d <source_directory>     # Specify directory to scan
    python findLangKeys.py -o <output_file>          # Specify output file
    python findLangKeys.py -h                        # Show help message
    python findLangKeys.py -v                        # Show version information
"""

import argparse
import os
import re
import sys
from pathlib import Path

__version__ = "1.0.0"
__author__ = "QuickWinstall Development Team"

# Default paths relative to script location
SCRIPT_DIR = Path(__file__).parent.parent
DEFAULT_SOURCE_DIR = SCRIPT_DIR / "src"
DEFAULT_OUTPUT_FILE = Path(__file__).parent / "presented_langKeys.txt"

# Regex patterns to match lang.GetString("key") calls
# Matches: lang.GetString("key"), LangManager.Instance.GetString("key"), etc.
LANG_KEY_PATTERNS = [
    r'lang\.GetString\s*\(\s*["\']([^"\']+)["\']\s*\)',
    r'LangManager\.Instance\.GetString\s*\(\s*["\']([^"\']+)["\']\s*\)',
    r'_langManager\.GetString\s*\(\s*["\']([^"\']+)["\']\s*\)',
]


def find_source_files(source_dir):
    """Find all C# source files in directory and subdirectories.
    
    Args:
        source_dir: Path to directory to search
        
    Returns:
        list: List of Path objects for found .cs files
    """
    source_path = Path(source_dir)
    
    if not source_path.exists():
        print(f"Error: Directory does not exist: {source_path}", file=sys.stderr)
        return []
    
    if not source_path.is_dir():
        print(f"Error: Not a directory: {source_path}", file=sys.stderr)
        return []
    
    # Find all .cs files recursively
    cs_files = list(source_path.rglob("*.cs"))
    
    # Sort by path for consistent processing
    cs_files.sort()
    
    return cs_files


def extract_lang_keys(file_path):
    """Extract localization keys from a source file.
    
    Args:
        file_path: Path to source file
        
    Returns:
        list: List of tuples (key, line_number)
    """
    keys = []
    
    try:
        with open(file_path, "r", encoding="utf-8") as f:
            for line_num, line in enumerate(f, 1):
                # Try each pattern
                for pattern in LANG_KEY_PATTERNS:
                    matches = re.finditer(pattern, line)
                    for match in matches:
                        key = match.group(1)
                        keys.append((key, line_num))
        
    except Exception as e:
        print(f"Warning: Failed to read {file_path}: {e}", file=sys.stderr)
    
    return keys


def scan_source_files(source_files, source_dir):
    """Scan all source files and extract localization keys.
    
    Args:
        source_files: List of Path objects
        source_dir: Base source directory for relative paths
        
    Returns:
        dict: Dictionary mapping keys to list of (file, line_number) tuples
    """
    all_keys = {}
    
    for source_file in source_files:
        rel_path = source_file.relative_to(source_dir)
        keys = extract_lang_keys(source_file)
        
        for key, line_num in keys:
            if key not in all_keys:
                all_keys[key] = []
            all_keys[key].append((str(rel_path), line_num))
    
    return all_keys


def write_results(keys_dict, output_file, source_dir):
    """Write found localization keys to output file.
    
    Args:
        keys_dict: Dictionary of keys and their locations
        output_file: Path to output file
        source_dir: Base source directory
        
    Returns:
        bool: True if write successful, False otherwise
    """
    try:
        # Create output directory if it doesn't exist
        output_file.parent.mkdir(parents=True, exist_ok=True)
        
        # Sort keys alphabetically
        sorted_keys = sorted(keys_dict.keys())
        
        with open(output_file, "w", encoding="utf-8") as f:
            # Write header
            f.write("# Localization Keys Found\n")
            f.write(f"# Source Directory: {source_dir}\n")
            f.write(f"# Total Unique Keys: {len(sorted_keys)}\n")
            f.write("#" + "=" * 70 + "\n\n")
            
            # Write each key with its locations
            for key in sorted_keys:
                f.write(f"{key}\n")
                locations = keys_dict[key]
                for file_path, line_num in locations:
                    f.write(f"  {file_path}:{line_num}\n")
                f.write("\n")
        
        return True
        
    except Exception as e:
        print(f"Error writing to output file: {e}", file=sys.stderr)
        return False


def main():
    """Main entry point for the script."""
    parser = argparse.ArgumentParser(
        description="Extract localization keys from source code",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  %(prog)s                              Scan default source directory
  %(prog)s -d src                       Scan specified directory
  %(prog)s -o keys_list.txt             Specify output file
  %(prog)s -d src -o output.txt         Custom directory and output
        """
    )
    
    parser.add_argument(
        "-d", "--directory",
        type=str,
        default=str(DEFAULT_SOURCE_DIR),
        help=f"Source directory to scan (default: {DEFAULT_SOURCE_DIR})"
    )
    
    parser.add_argument(
        "-o", "--output",
        type=str,
        default=str(DEFAULT_OUTPUT_FILE),
        help=f"Output file for results (default: {DEFAULT_OUTPUT_FILE})"
    )
    
    parser.add_argument(
        "-v", "--version",
        action="version",
        version=f"%(prog)s {__version__}"
    )
    
    args = parser.parse_args()
    
    source_dir = Path(args.directory)
    output_file = Path(args.output)
    
    print(f"Scanning for localization keys in: {source_dir}")
    print()
    
    # Find all source files
    source_files = find_source_files(source_dir)
    
    if not source_files:
        print("No source files found.")
        return 0
    
    print(f"Found {len(source_files)} source file(s)")
    print("Extracting localization keys...")
    print()
    
    # Scan files and extract keys
    keys_dict = scan_source_files(source_files, source_dir)
    
    if not keys_dict:
        print("No localization keys found.")
        return 0
    
    # Calculate total occurrences
    total_occurrences = sum(len(locations) for locations in keys_dict.values())
    
    print(f"Found {len(keys_dict)} unique localization key(s)")
    print(f"Total occurrences: {total_occurrences}")
    print()
    
    # Display some sample keys
    sorted_keys = sorted(keys_dict.keys())
    sample_count = min(10, len(sorted_keys))
    print(f"Sample keys (showing {sample_count} of {len(sorted_keys)}):")
    for key in sorted_keys[:sample_count]:
        count = len(keys_dict[key])
        print(f"  - {key} ({count} occurrence{'s' if count > 1 else ''})")
    
    if len(sorted_keys) > sample_count:
        print(f"  ... and {len(sorted_keys) - sample_count} more")
    
    print()
    print(f"Writing results to: {output_file}")
    
    # Write results to file
    if write_results(keys_dict, output_file, source_dir):
        print(f"✓ Results written successfully to {output_file}")
        return 0
    else:
        print(f"✗ Failed to write results", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
