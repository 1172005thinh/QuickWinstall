#!/usr/bin/env python3
"""
compareResources.py - Compare resource usage against available resources

This script compares localization keys and icons referenced in the code with
the actual resources available in the project. It identifies missing resources,
unused resources, and generates a comprehensive report.

Requirements:
- Python 3.x
- No additional libraries required

Usage:
    python compareResources.py                       # Compare both keys and icons
    python compareResources.py --keys-only           # Compare only localization keys
    python compareResources.py --icons-only          # Compare only icons
    python compareResources.py -o <output_file>      # Specify output file
    python compareResources.py -h                    # Show help message
    python compareResources.py -v                    # Show version information
"""

import argparse
import json
import os
import re
import sys
from pathlib import Path

__version__ = "1.0.0"
__author__ = "QuickWinstall Development Team"

# Default paths relative to script location
SCRIPT_DIR = Path(__file__).parent.parent
PRESENTED_KEYS_FILE = Path(__file__).parent / "presented_langKeys.txt"
PRESENTED_ICONS_FILE = Path(__file__).parent / "presented_icons.txt"
LANG_FILES_DIR = SCRIPT_DIR / "res" / "langs"
ICONS_DIR = SCRIPT_DIR / "res" / "icons"
DEFAULT_OUTPUT_FILE = Path(__file__).parent / "resource_comparison_report.txt"


def parse_presented_keys(file_path):
    """Parse presented_langKeys.txt to extract keys and their locations.
    
    Args:
        file_path: Path to presented_langKeys.txt
        
    Returns:
        dict: Dictionary mapping keys to list of locations
    """
    keys = {}
    
    try:
        with open(file_path, "r", encoding="utf-8") as f:
            current_key = None
            for line in f:
                line = line.rstrip()
                
                # Skip header lines
                if line.startswith("#") or not line:
                    continue
                
                # Check if this is a key line (no leading whitespace)
                if not line.startswith(" "):
                    current_key = line.strip()
                    keys[current_key] = []
                elif current_key and line.startswith("  "):
                    # This is a location line
                    location = line.strip()
                    keys[current_key].append(location)
    
    except FileNotFoundError:
        print(f"Error: File not found: {file_path}", file=sys.stderr)
        print("Please run 'python findLangKeys.py' first.", file=sys.stderr)
        return None
    except Exception as e:
        print(f"Error parsing {file_path}: {e}", file=sys.stderr)
        return None
    
    return keys


def parse_presented_icons(file_path):
    """Parse presented_icons.txt to extract icon names.
    
    Args:
        file_path: Path to presented_icons.txt
        
    Returns:
        set: Set of icon names (without extension)
    """
    icons = set()
    
    try:
        with open(file_path, "r", encoding="utf-8") as f:
            for line in f:
                line = line.strip()
                
                # Skip header lines and empty lines
                if line.startswith("#") or not line or line.startswith("Size:"):
                    continue
                
                # Extract filename
                icon_path = Path(line)
                icon_name = icon_path.stem  # Filename without extension
                icons.add(icon_name)
    
    except FileNotFoundError:
        print(f"Error: File not found: {file_path}", file=sys.stderr)
        print("Please run 'python findIcons.py' first.", file=sys.stderr)
        return None
    except Exception as e:
        print(f"Error parsing {file_path}: {e}", file=sys.stderr)
        return None
    
    return icons


def get_available_lang_keys(lang_dir):
    """Get all available localization keys from language JSON files.
    
    Args:
        lang_dir: Directory containing language JSON files
        
    Returns:
        dict: Dictionary mapping keys to list of files they appear in
    """
    available_keys = {}
    
    try:
        json_files = list(Path(lang_dir).glob("*.json"))
        
        for json_file in json_files:
            try:
                with open(json_file, "r", encoding="utf-8") as f:
                    data = json.load(f)
                
                # Flatten nested keys
                def flatten_dict(d, parent_key=""):
                    items = []
                    for k, v in d.items():
                        new_key = f"{parent_key}.{k}" if parent_key else k
                        if isinstance(v, dict):
                            items.extend(flatten_dict(v, new_key))
                        else:
                            items.append(new_key)
                    return items
                
                keys = flatten_dict(data)
                for key in keys:
                    if key not in available_keys:
                        available_keys[key] = []
                    available_keys[key].append(json_file.name)
            
            except json.JSONDecodeError as e:
                print(f"Warning: Failed to parse {json_file.name}: {e}", file=sys.stderr)
            except Exception as e:
                print(f"Warning: Error reading {json_file.name}: {e}", file=sys.stderr)
    
    except Exception as e:
        print(f"Error accessing language directory: {e}", file=sys.stderr)
        return None
    
    return available_keys


def get_available_icons(icons_dir):
    """Get all available icon names from icons directory.
    
    Args:
        icons_dir: Directory containing icon files
        
    Returns:
        set: Set of icon names (without extension)
    """
    icons = set()
    
    try:
        ico_files = list(Path(icons_dir).glob("*.ico"))
        for ico_file in ico_files:
            icons.add(ico_file.stem)
    except Exception as e:
        print(f"Error accessing icons directory: {e}", file=sys.stderr)
        return None
    
    return icons


def compare_keys(used_keys, available_keys):
    """Compare used keys with available keys.
    
    Args:
        used_keys: Dictionary of keys used in code
        available_keys: Dictionary of keys available in language files
        
    Returns:
        dict: Dictionary with 'missing' and 'unused' lists
    """
    missing = []
    unused = []
    
    used_set = set(used_keys.keys())
    available_set = set(available_keys.keys())
    
    # Find missing keys (used but not available)
    missing_keys = used_set - available_set
    for key in sorted(missing_keys):
        locations = used_keys[key]
        missing.append({
            "key": key,
            "locations": locations
        })
    
    # Find unused keys (available but not used)
    unused_keys = available_set - used_set
    for key in sorted(unused_keys):
        files = available_keys[key]
        unused.append({
            "key": key,
            "files": files
        })
    
    return {
        "missing": missing,
        "unused": unused,
        "total_used": len(used_set),
        "total_available": len(available_set),
        "total_missing": len(missing),
        "total_unused": len(unused)
    }


def compare_icons(used_icons, available_icons):
    """Compare used icons with available icons.
    
    Args:
        used_icons: Set of icon names used in code
        available_icons: Set of icon names available in icons directory
        
    Returns:
        dict: Dictionary with 'missing' and 'unused' lists
    """
    missing = sorted(used_icons - available_icons)
    unused = sorted(available_icons - used_icons)
    
    return {
        "missing": missing,
        "unused": unused,
        "total_used": len(used_icons),
        "total_available": len(available_icons),
        "total_missing": len(missing),
        "total_unused": len(unused)
    }


def write_report(output_file, keys_comparison=None, icons_comparison=None):
    """Write comparison report to file.
    
    Args:
        output_file: Path to output file
        keys_comparison: Dictionary with keys comparison results
        icons_comparison: Dictionary with icons comparison results
        
    Returns:
        bool: True if write successful, False otherwise
    """
    try:
        with open(output_file, "w", encoding="utf-8") as f:
            f.write("=" * 80 + "\n")
            f.write("RESOURCE COMPARISON REPORT\n")
            f.write("=" * 80 + "\n\n")
            
            # Localization Keys Section
            if keys_comparison:
                f.write("LOCALIZATION KEYS\n")
                f.write("-" * 80 + "\n\n")
                
                f.write(f"Total keys used in code:           {keys_comparison['total_used']}\n")
                f.write(f"Total keys available in resources: {keys_comparison['total_available']}\n")
                f.write(f"Missing keys:                      {keys_comparison['total_missing']}\n")
                f.write(f"Unused keys:                       {keys_comparison['total_unused']}\n\n")
                
                # Missing keys
                if keys_comparison['missing']:
                    f.write("MISSING LOCALIZATION KEYS (Used in code but not in language files)\n")
                    f.write("-" * 80 + "\n")
                    for item in keys_comparison['missing']:
                        f.write(f"\n{item['key']}\n")
                        for location in item['locations']:
                            f.write(f"  {location}\n")
                    f.write("\n")
                else:
                    f.write("✓ No missing localization keys.\n\n")
                
                # Unused keys
                if keys_comparison['unused']:
                    f.write("UNUSED LOCALIZATION KEYS (Available but not used in code)\n")
                    f.write("-" * 80 + "\n")
                    for item in keys_comparison['unused']:
                        f.write(f"\n{item['key']}\n")
                        for file_name in item['files']:
                            f.write(f"  {file_name}\n")
                    f.write("\n")
                else:
                    f.write("✓ No unused localization keys.\n\n")
                
                f.write("\n")
            
            # Icons Section
            if icons_comparison:
                f.write("ICONS\n")
                f.write("-" * 80 + "\n\n")
                
                f.write(f"Total icons referenced in code:    {icons_comparison['total_used']}\n")
                f.write(f"Total icons available in resources: {icons_comparison['total_available']}\n")
                f.write(f"Missing icons:                     {icons_comparison['total_missing']}\n")
                f.write(f"Unused icons:                      {icons_comparison['total_unused']}\n\n")
                
                # Missing icons
                if icons_comparison['missing']:
                    f.write("MISSING ICONS (Referenced in code but not in icons directory)\n")
                    f.write("-" * 80 + "\n")
                    for icon in icons_comparison['missing']:
                        f.write(f"  {icon}.ico\n")
                    f.write("\n")
                else:
                    f.write("✓ No missing icons.\n\n")
                
                # Unused icons
                if icons_comparison['unused']:
                    f.write("UNUSED ICONS (Available but not referenced in code)\n")
                    f.write("-" * 80 + "\n")
                    for icon in icons_comparison['unused']:
                        f.write(f"  {icon}.ico\n")
                    f.write("\n")
                else:
                    f.write("✓ No unused icons.\n\n")
            
            f.write("=" * 80 + "\n")
            f.write("END OF REPORT\n")
            f.write("=" * 80 + "\n")
        
        return True
    
    except Exception as e:
        print(f"Error writing report: {e}", file=sys.stderr)
        return False


def main():
    """Main entry point for the script."""
    parser = argparse.ArgumentParser(
        description="Compare resource usage against available resources",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  %(prog)s                              Compare both keys and icons
  %(prog)s --keys-only                  Compare only localization keys
  %(prog)s --icons-only                 Compare only icons
  %(prog)s -o report.txt                Specify output file
        """
    )
    
    parser.add_argument(
        "--keys-only",
        action="store_true",
        help="Compare only localization keys"
    )
    
    parser.add_argument(
        "--icons-only",
        action="store_true",
        help="Compare only icons"
    )
    
    parser.add_argument(
        "-o", "--output",
        type=str,
        default=str(DEFAULT_OUTPUT_FILE),
        help=f"Output file for report (default: {DEFAULT_OUTPUT_FILE})"
    )
    
    parser.add_argument(
        "-v", "--version",
        action="version",
        version=f"%(prog)s {__version__}"
    )
    
    args = parser.parse_args()
    
    output_file = Path(args.output)
    
    # Determine what to compare
    compare_keys_flag = not args.icons_only
    compare_icons_flag = not args.keys_only
    
    keys_comparison = None
    icons_comparison = None
    
    # Compare localization keys
    if compare_keys_flag:
        print("Comparing localization keys...")
        
        used_keys = parse_presented_keys(PRESENTED_KEYS_FILE)
        if used_keys is None:
            return 1
        
        available_keys = get_available_lang_keys(LANG_FILES_DIR)
        if available_keys is None:
            return 1
        
        keys_comparison = compare_keys(used_keys, available_keys)
        
        print(f"  Used in code:     {keys_comparison['total_used']}")
        print(f"  Available:        {keys_comparison['total_available']}")
        print(f"  Missing:          {keys_comparison['total_missing']}")
        print(f"  Unused:           {keys_comparison['total_unused']}")
        print()
    
    # Compare icons
    if compare_icons_flag:
        print("Comparing icons...")
        
        used_icons = parse_presented_icons(PRESENTED_ICONS_FILE)
        if used_icons is None:
            return 1
        
        available_icons = get_available_icons(ICONS_DIR)
        if available_icons is None:
            return 1
        
        icons_comparison = compare_icons(used_icons, available_icons)
        
        print(f"  Referenced:       {icons_comparison['total_used']}")
        print(f"  Available:        {icons_comparison['total_available']}")
        print(f"  Missing:          {icons_comparison['total_missing']}")
        print(f"  Unused:           {icons_comparison['total_unused']}")
        print()
    
    # Write report
    print(f"Writing report to: {output_file}")
    if write_report(output_file, keys_comparison, icons_comparison):
        print(f"✓ Report written successfully")
        
        # Display summary
        has_issues = False
        if keys_comparison and (keys_comparison['total_missing'] > 0 or keys_comparison['total_unused'] > 0):
            has_issues = True
        if icons_comparison and (icons_comparison['total_missing'] > 0 or icons_comparison['total_unused'] > 0):
            has_issues = True
        
        if has_issues:
            print("\n⚠ Issues found. Please review the report for details.")
            return 1
        else:
            print("\n✓ No issues found. All resources are properly matched.")
            return 0
    else:
        print(f"✗ Failed to write report", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
