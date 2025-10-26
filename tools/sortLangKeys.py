#!/usr/bin/env python3
"""
sortLangKeys.py - Sort localization keys in language JSON files

This script sorts the keys in language JSON files alphabetically while
preserving the structure and formatting. Useful for maintaining organized
language files and making diffs clearer.

Requirements:
- Python 3.x
- No additional libraries required (uses standard json module)

Usage:
    python sortLangKeys.py                           # Sort all language files
    python sortLangKeys.py -f <file>                 # Sort specific file
    python sortLangKeys.py -d <directory>            # Sort files in directory
    python sortLangKeys.py --check                   # Check if files are sorted
    python sortLangKeys.py --dry-run                 # Preview changes without writing
    python sortLangKeys.py -h                        # Show help message
    python sortLangKeys.py -v                        # Show version information
"""

import argparse
import json
import sys
from pathlib import Path

__version__ = "1.0.0"
__author__ = "QuickWinstall Development Team"

# Default paths relative to script location
SCRIPT_DIR = Path(__file__).parent.parent
DEFAULT_LANG_DIR = SCRIPT_DIR / "res" / "langs"


def sort_dict_recursive(obj):
    """Recursively sort dictionary keys.
    
    Args:
        obj: Object to sort (dict, list, or other)
        
    Returns:
        Sorted object with same structure
    """
    if isinstance(obj, dict):
        return {k: sort_dict_recursive(v) for k, v in sorted(obj.items())}
    elif isinstance(obj, list):
        return [sort_dict_recursive(item) for item in obj]
    else:
        return obj


def is_sorted(obj):
    """Check if dictionary keys are sorted recursively.
    
    Args:
        obj: Object to check
        
    Returns:
        bool: True if sorted, False otherwise
    """
    if isinstance(obj, dict):
        keys = list(obj.keys())
        if keys != sorted(keys):
            return False
        return all(is_sorted(v) for v in obj.values())
    elif isinstance(obj, list):
        return all(is_sorted(item) for item in obj)
    return True


def sort_lang_file(file_path, dry_run=False, check_only=False):
    """Sort keys in a language JSON file.
    
    Args:
        file_path: Path to JSON file
        dry_run: If True, don't write changes
        check_only: If True, only check if sorted
        
    Returns:
        dict: Result with 'status', 'message', and 'changed' keys
    """
    try:
        # Read the file
        with open(file_path, "r", encoding="utf-8") as f:
            data = json.load(f)
        
        # Check if already sorted
        already_sorted = is_sorted(data)
        
        if check_only:
            if already_sorted:
                return {
                    "status": "ok",
                    "message": "Already sorted",
                    "changed": False
                }
            else:
                return {
                    "status": "unsorted",
                    "message": "Not sorted",
                    "changed": False
                }
        
        if already_sorted:
            return {
                "status": "ok",
                "message": "Already sorted, no changes needed",
                "changed": False
            }
        
        # Sort the data
        sorted_data = sort_dict_recursive(data)
        
        if dry_run:
            return {
                "status": "would_change",
                "message": "Would be sorted",
                "changed": True
            }
        
        # Write back to file with nice formatting
        with open(file_path, "w", encoding="utf-8") as f:
            json.dump(sorted_data, f, indent=2, ensure_ascii=False)
            f.write("\n")  # Add trailing newline
        
        return {
            "status": "sorted",
            "message": "Sorted successfully",
            "changed": True
        }
        
    except json.JSONDecodeError as e:
        return {
            "status": "error",
            "message": f"Invalid JSON: {e}",
            "changed": False
        }
    except Exception as e:
        return {
            "status": "error",
            "message": f"Error: {e}",
            "changed": False
        }


def find_lang_files(directory):
    """Find all JSON files in language directory.
    
    Args:
        directory: Path to directory
        
    Returns:
        list: List of Path objects for JSON files
    """
    dir_path = Path(directory)
    
    if not dir_path.exists():
        print(f"Error: Directory does not exist: {dir_path}", file=sys.stderr)
        return []
    
    if not dir_path.is_dir():
        print(f"Error: Not a directory: {dir_path}", file=sys.stderr)
        return []
    
    # Find all .json files
    json_files = list(dir_path.glob("*.json"))
    json_files.sort()
    
    return json_files


def main():
    """Main entry point for the script."""
    parser = argparse.ArgumentParser(
        description="Sort localization keys in language JSON files",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  %(prog)s                              Sort all language files
  %(prog)s -f en-US.json                Sort specific file
  %(prog)s -d res/langs                 Sort files in directory
  %(prog)s --check                      Check if files are sorted
  %(prog)s --dry-run                    Preview changes
        """
    )
    
    parser.add_argument(
        "-f", "--file",
        type=str,
        help="Specific JSON file to sort"
    )
    
    parser.add_argument(
        "-d", "--directory",
        type=str,
        default=str(DEFAULT_LANG_DIR),
        help=f"Directory containing language files (default: {DEFAULT_LANG_DIR})"
    )
    
    parser.add_argument(
        "--check",
        action="store_true",
        help="Check if files are sorted without modifying them"
    )
    
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="Preview changes without writing files"
    )
    
    parser.add_argument(
        "-v", "--version",
        action="version",
        version=f"%(prog)s {__version__}"
    )
    
    args = parser.parse_args()
    
    # Determine files to process
    if args.file:
        files = [Path(args.file)]
    else:
        files = find_lang_files(args.directory)
    
    if not files:
        print("No JSON files found to process.")
        return 0
    
    # Print mode
    if args.check:
        print("Checking if files are sorted...")
    elif args.dry_run:
        print("[DRY RUN MODE - No files will be modified]")
    print()
    
    # Process each file
    results = {
        "ok": 0,
        "sorted": 0,
        "would_change": 0,
        "unsorted": 0,
        "error": 0
    }
    
    for file_path in files:
        result = sort_lang_file(file_path, args.dry_run, args.check)
        status = result["status"]
        message = result["message"]
        
        # Update counters
        results[status] = results.get(status, 0) + 1
        
        # Print result with appropriate icon
        if status == "ok":
            print(f"✓ {file_path.name}: {message}")
        elif status == "sorted":
            print(f"✓ {file_path.name}: {message}")
        elif status == "would_change":
            print(f"• {file_path.name}: {message}")
        elif status == "unsorted":
            print(f"✗ {file_path.name}: {message}")
        elif status == "error":
            print(f"✗ {file_path.name}: {message}")
    
    # Print summary
    print()
    if args.check:
        sorted_count = results["ok"]
        unsorted_count = results["unsorted"]
        error_count = results["error"]
        
        print(f"Check completed:")
        print(f"  ✓ Sorted:   {sorted_count}")
        if unsorted_count > 0:
            print(f"  ✗ Unsorted: {unsorted_count}")
        if error_count > 0:
            print(f"  ✗ Errors:   {error_count}")
        
        return 0 if unsorted_count == 0 and error_count == 0 else 1
    
    elif args.dry_run:
        no_change = results["ok"]
        would_change = results["would_change"]
        error_count = results["error"]
        
        print(f"Dry run completed:")
        print(f"  ✓ Already sorted: {no_change}")
        if would_change > 0:
            print(f"  • Would sort:     {would_change}")
        if error_count > 0:
            print(f"  ✗ Errors:         {error_count}")
        
        return 0
    
    else:
        no_change = results["ok"]
        sorted_count = results["sorted"]
        error_count = results["error"]
        
        print(f"Sort completed:")
        print(f"  ✓ No changes needed: {no_change}")
        if sorted_count > 0:
            print(f"  ✓ Sorted:            {sorted_count}")
        if error_count > 0:
            print(f"  ✗ Errors:            {error_count}")
        
        return 0 if error_count == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
