#!/usr/bin/env python3
"""
findIcons.py - Search for icon files and output their paths

This script searches for all .ico files in a specified directory and its
subdirectories, then outputs the paths to a text file.

Requirements:
- Python 3.x
- No additional libraries required

Usage:
    python findIcons.py                              # Search in default directory
    python findIcons.py -d <search_directory>        # Specify directory to search
    python findIcons.py -o <output_file>             # Specify output file
    python findIcons.py -h                           # Show help message
    python findIcons.py -v                           # Show version information
"""

import argparse
import os
import sys
from pathlib import Path

__version__ = "1.0.0"
__author__ = "QuickWinstall Development Team"

# Default paths relative to script location
SCRIPT_DIR = Path(__file__).parent.parent
DEFAULT_SEARCH_DIR = SCRIPT_DIR / "res" / "icons"
DEFAULT_OUTPUT_FILE = Path(__file__).parent / "presented_icons.txt"


def find_icon_files(search_dir):
    """Find all .ico files in directory and subdirectories.
    
    Args:
        search_dir: Path to directory to search
        
    Returns:
        list: List of Path objects for found .ico files
    """
    search_path = Path(search_dir)
    
    if not search_path.exists():
        print(f"Error: Directory does not exist: {search_path}", file=sys.stderr)
        return []
    
    if not search_path.is_dir():
        print(f"Error: Not a directory: {search_path}", file=sys.stderr)
        return []
    
    # Find all .ico files recursively
    ico_files = list(search_path.rglob("*.ico"))
    
    # Sort by path for consistent output
    ico_files.sort()
    
    return ico_files


def get_relative_path(file_path, base_path):
    """Get relative path from base directory.
    
    Args:
        file_path: Path to file
        base_path: Base directory path
        
    Returns:
        str: Relative path string
    """
    try:
        return str(file_path.relative_to(base_path))
    except ValueError:
        # If paths don't share common base, return absolute path
        return str(file_path.absolute())


def write_results(ico_files, output_file, search_dir):
    """Write found icon paths to output file.
    
    Args:
        ico_files: List of Path objects
        output_file: Path to output file
        search_dir: Base search directory for relative paths
        
    Returns:
        bool: True if write successful, False otherwise
    """
    try:
        # Create output directory if it doesn't exist
        output_file.parent.mkdir(parents=True, exist_ok=True)
        
        with open(output_file, "w", encoding="utf-8") as f:
            # Write header
            f.write("# Icon Files Found\n")
            f.write(f"# Search Directory: {search_dir}\n")
            f.write(f"# Total Files: {len(ico_files)}\n")
            f.write("#" + "=" * 70 + "\n\n")
            
            # Write each icon path
            for ico_file in ico_files:
                # Get relative path from search directory
                rel_path = get_relative_path(ico_file, search_dir)
                
                # Get file size
                try:
                    size_bytes = ico_file.stat().st_size
                    size_kb = size_bytes / 1024
                    size_str = f"{size_kb:.1f} KB"
                except:
                    size_str = "Unknown"
                
                # Write path and size
                f.write(f"{rel_path}\n")
                f.write(f"  Size: {size_str}\n\n")
        
        return True
        
    except Exception as e:
        print(f"Error writing to output file: {e}", file=sys.stderr)
        return False


def main():
    """Main entry point for the script."""
    parser = argparse.ArgumentParser(
        description="Search for .ico files and output their paths",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  %(prog)s                              Search in default directory
  %(prog)s -d res/icons                 Search in specified directory
  %(prog)s -o icons_list.txt            Specify output file
  %(prog)s -d res -o output.txt         Custom directory and output
        """
    )
    
    parser.add_argument(
        "-d", "--directory",
        type=str,
        default=str(DEFAULT_SEARCH_DIR),
        help=f"Directory to search for icon files (default: {DEFAULT_SEARCH_DIR})"
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
    
    search_dir = Path(args.directory)
    output_file = Path(args.output)
    
    print(f"Searching for .ico files in: {search_dir}")
    print()
    
    # Find all icon files
    ico_files = find_icon_files(search_dir)
    
    if not ico_files:
        print("No .ico files found.")
        return 0
    
    print(f"Found {len(ico_files)} icon file(s):")
    for ico_file in ico_files:
        rel_path = get_relative_path(ico_file, search_dir)
        print(f"  - {rel_path}")
    
    print()
    print(f"Writing results to: {output_file}")
    
    # Write results to file
    if write_results(ico_files, output_file, search_dir):
        print(f"✓ Results written successfully to {output_file}")
        return 0
    else:
        print(f"✗ Failed to write results", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
