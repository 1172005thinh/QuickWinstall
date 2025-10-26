#!/usr/bin/env python3
"""
png2ico.py - Convert PNG images to ICO format using ffmpeg

This script converts PNG images to ICO format with automatic size detection:
- Files without '256' in name -> 16x16 icons
- Files with '256' in name -> 256x256 icons

Requirements:
- Python 3.x
- ffmpeg installed and added to system PATH

Usage:
    python png2ico.py                    # Convert all PNGs in default input folder
    python png2ico.py -i <input_folder>  # Specify input folder
    python png2ico.py -o <output_folder> # Specify output folder
    python png2ico.py -s <size>          # Convert only specific size (16 or 256)
    python png2ico.py -c                 # Clean all .ico files in output folder
    python png2ico.py --dry-run          # Simulate conversion without creating files
    python png2ico.py -h                 # Show help message
    python png2ico.py -v                 # Show version information
"""

import argparse
import os
import subprocess
import sys
from pathlib import Path

__version__ = "1.0.0"
__author__ = "QuickWinstall Development Team"

# Default paths relative to script location
SCRIPT_DIR = Path(__file__).parent.parent
DEFAULT_INPUT = SCRIPT_DIR / "res" / "icons" / "pngs"
DEFAULT_OUTPUT = SCRIPT_DIR / "res" / "icons"


def check_ffmpeg():
    """Check if ffmpeg is installed and accessible."""
    try:
        subprocess.run(
            ["ffmpeg", "-version"],
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
            check=True
        )
        return True
    except (subprocess.CalledProcessError, FileNotFoundError):
        return False


def get_icon_size(filename):
    """Determine icon size based on filename.
    
    Args:
        filename: Name of the PNG file
        
    Returns:
        int: Icon size (16 or 256)
    """
    return 256 if "256" in filename else 16


def convert_png_to_ico(input_path, output_path, size, dry_run=False):
    """Convert a single PNG file to ICO format.
    
    Args:
        input_path: Path to input PNG file
        output_path: Path to output ICO file
        size: Icon size (16 or 256)
        dry_run: If True, simulate without creating files
        
    Returns:
        bool: True if conversion successful, False otherwise
    """
    if dry_run:
        print(f"[DRY RUN] Would convert: {input_path.name} -> {output_path.name} ({size}x{size})")
        return True
    
    try:
        # Create output directory if it doesn't exist
        output_path.parent.mkdir(parents=True, exist_ok=True)
        
        # Run ffmpeg conversion
        cmd = [
            "ffmpeg",
            "-i", str(input_path),
            "-vf", f"scale={size}:{size}",
            "-y",  # Overwrite output file if exists
            str(output_path)
        ]
        
        result = subprocess.run(
            cmd,
            stdout=subprocess.DEVNULL,
            stderr=subprocess.PIPE,
            check=True
        )
        
        print(f"✓ Converted: {input_path.name} -> {output_path.name} ({size}x{size})")
        return True
        
    except subprocess.CalledProcessError as e:
        print(f"✗ Failed to convert {input_path.name}: {e.stderr.decode()}", file=sys.stderr)
        return False
    except Exception as e:
        print(f"✗ Error converting {input_path.name}: {e}", file=sys.stderr)
        return False


def clean_ico_files(output_dir, dry_run=False):
    """Remove all .ico files from output directory.
    
    Args:
        output_dir: Directory to clean
        dry_run: If True, simulate without deleting files
        
    Returns:
        int: Number of files deleted
    """
    output_path = Path(output_dir)
    
    if not output_path.exists():
        print(f"Output directory does not exist: {output_path}")
        return 0
    
    ico_files = list(output_path.glob("*.ico"))
    
    if not ico_files:
        print("No .ico files found to clean.")
        return 0
    
    print(f"Found {len(ico_files)} .ico file(s) in {output_path}")
    
    if not dry_run:
        # Confirmation prompt
        response = input(f"Are you sure you want to delete {len(ico_files)} .ico file(s)? (yes/no): ")
        if response.lower() not in ["yes", "y"]:
            print("Clean operation cancelled.")
            return 0
    
    deleted_count = 0
    for ico_file in ico_files:
        if dry_run:
            print(f"[DRY RUN] Would delete: {ico_file.name}")
            deleted_count += 1
        else:
            try:
                ico_file.unlink()
                print(f"✓ Deleted: {ico_file.name}")
                deleted_count += 1
            except Exception as e:
                print(f"✗ Failed to delete {ico_file.name}: {e}", file=sys.stderr)
    
    return deleted_count


def main():
    """Main entry point for the script."""
    parser = argparse.ArgumentParser(
        description="Convert PNG images to ICO format using ffmpeg",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  %(prog)s                              Convert all PNGs in default folder
  %(prog)s -i icons/pngs -o icons      Specify input and output folders
  %(prog)s -s 16                       Convert only 16x16 icons
  %(prog)s -s 256                      Convert only 256x256 icons
  %(prog)s -c                          Clean all .ico files
  %(prog)s --dry-run                   Simulate conversion
        """
    )
    
    parser.add_argument(
        "-i", "--input",
        type=str,
        default=str(DEFAULT_INPUT),
        help=f"Input folder containing PNG files (default: {DEFAULT_INPUT})"
    )
    
    parser.add_argument(
        "-o", "--output",
        type=str,
        default=str(DEFAULT_OUTPUT),
        help=f"Output folder for ICO files (default: {DEFAULT_OUTPUT})"
    )
    
    parser.add_argument(
        "-s", "--size",
        type=int,
        choices=[16, 256],
        help="Convert only specific size (16 or 256)"
    )
    
    parser.add_argument(
        "-c", "--clean",
        action="store_true",
        help="Clean all .ico files in output folder"
    )
    
    parser.add_argument(
        "--dry-run",
        action="store_true",
        help="Simulate conversion without creating files"
    )
    
    parser.add_argument(
        "-v", "--version",
        action="version",
        version=f"%(prog)s {__version__}"
    )
    
    args = parser.parse_args()
    
    # Handle clean operation
    if args.clean:
        deleted_count = clean_ico_files(args.output, args.dry_run)
        print(f"\nCleaning completed: {deleted_count} file(s) {'would be ' if args.dry_run else ''}deleted.")
        return 0
    
    # Check if ffmpeg is available
    if not check_ffmpeg():
        print("Error: ffmpeg is not installed or not in system PATH.", file=sys.stderr)
        print("Please install ffmpeg and add it to your system PATH.", file=sys.stderr)
        return 1
    
    input_path = Path(args.input)
    output_path = Path(args.output)
    
    # Validate input directory
    if not input_path.exists():
        print(f"Error: Input directory does not exist: {input_path}", file=sys.stderr)
        return 1
    
    # Find all PNG files
    png_files = list(input_path.glob("*.png"))
    
    if not png_files:
        print(f"No PNG files found in {input_path}")
        return 0
    
    # Filter by size if specified
    if args.size:
        png_files = [f for f in png_files if get_icon_size(f.name) == args.size]
        if not png_files:
            print(f"No PNG files found for size {args.size}x{args.size}")
            return 0
    
    print(f"Found {len(png_files)} PNG file(s) to convert")
    if args.dry_run:
        print("[DRY RUN MODE - No files will be created]\n")
    else:
        print()
    
    # Convert each PNG file
    success_count = 0
    failed_count = 0
    
    for png_file in png_files:
        size = get_icon_size(png_file.name)
        ico_file = output_path / png_file.with_suffix(".ico").name
        
        if convert_png_to_ico(png_file, ico_file, size, args.dry_run):
            success_count += 1
        else:
            failed_count += 1
    
    # Summary
    print(f"\n{'Simulation' if args.dry_run else 'Conversion'} completed:")
    print(f"  ✓ Success: {success_count}")
    if failed_count > 0:
        print(f"  ✗ Failed:  {failed_count}")
    
    return 0 if failed_count == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
