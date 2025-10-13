#!/usr/bin/env python3
"""
Convert PNG files in res/icons/png/ to 16x16 .ico files in res/icons/ using ffmpeg.

Rules:
- Convert files named <name>.png (but NOT those that include '256' before the extension).
- Output is res/icons/<name>.ico with 16x16 size.

Options:
  --force    Overwrite existing .ico files
  --dry-run  Print actions without running ffmpeg
  --src      Override source directory (default: res/icons/png)
  --dst      Override destination directory (default: res/icons)

Requires ffmpeg available on PATH.
"""
import argparse
import subprocess
import sys
from pathlib import Path


def find_pngs(src_dir: Path):
    if not src_dir.exists():
        return []
    # match .png files that DON'T end with '256.png' (case-insensitive)
    return [p for p in src_dir.iterdir() if p.is_file() and p.suffix.lower() == '.png' and not p.stem.lower().endswith('256')]


def build_cmd(ffmpeg: str, src: Path, dst: Path):
    # ffmpeg -y -i input.png -vf scale=16:16 output.ico
    return [ffmpeg, '-y', '-i', str(src), '-vf', 'scale=16:16', str(dst)]


def main():
    parser = argparse.ArgumentParser(description='Convert PNGs to 16x16 ICO using ffmpeg')
    parser.add_argument('--force', action='store_true', help='Overwrite existing .ico files')
    parser.add_argument('--dry-run', action='store_true', help='Show actions but do not run ffmpeg')
    parser.add_argument('--src', type=Path, default=Path('res/icons/png'), help='Source directory')
    parser.add_argument('--dst', type=Path, default=Path('res/icons'), help='Destination directory')
    parser.add_argument('--ffmpeg', default='ffmpeg', help='ffmpeg executable (default: ffmpeg)')
    args = parser.parse_args()

    src_dir = args.src
    dst_dir = args.dst
    dst_dir.mkdir(parents=True, exist_ok=True)

    pngs = find_pngs(src_dir)
    if not pngs:
        print(f'No matching PNG files found in {src_dir!s}')
        return

    for p in sorted(pngs):
        out_name = p.stem + '.ico'
        out_path = dst_dir / out_name
        if out_path.exists() and not args.force:
            print(f'Skipping (exists): {out_path!s}')
            continue

        cmd = build_cmd(args.ffmpeg, p, out_path)
        print(('DRY-RUN: ' if args.dry_run else '') + ' '.join(map(str, cmd)))
        if args.dry_run:
            continue

        try:
            subprocess.run(cmd, check=True)
            print(f'Created: {out_path!s}')
        except subprocess.CalledProcessError as e:
            print(f'Failed to create {out_path!s}: {e}', file=sys.stderr)


if __name__ == '__main__':
    main()
