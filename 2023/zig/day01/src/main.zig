const std = @import("std");

pub fn main() !void {
    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    defer arena.deinit();
    const lineAllocator = arena.allocator();

    var list = std.ArrayList([]const u8).init(lineAllocator);
    // read input
    readInputFile("input.txt", &list) catch |err| {
        std.debug.print("Error: {}\n", .{err});
        return err;
    };

    // process
    const character_numbers: [10][]const u8 = .{ "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
    var numbers = std.ArrayList(u64).init(arena.allocator());
    for (list.items) |line| {
        // process line
        // std.debug.print("index: {d}\n", .{index});
        // std.debug.print("line: {s}\n", .{line});
        // find number on left side
        var num: [2]u8 = undefined;
        var left_is_string = false;
        var right_is_string = false;
        var left: usize = line.len;
        // check digit
        for (line, 0..) |c, index| {
            if (std.ascii.isDigit(c)) {
                // std.debug.print("found digit: {c}\n", .{c});
                left = index;
                break;
            }
        }
        // check digit in text
        for (character_numbers, 0..) |number, word_index| {
            const index = std.mem.indexOf(u8, line, number);
            if (index != null and index.? < left) {
                // std.debug.print("found number: {s}\n", .{number});
                left = index.?;
                left_is_string = true;
                num[0] = transformToDigit(character_numbers[word_index]);
                // std.debug.print("saved number: {d}\n", .{num[0]});
            }
        }

        var right: usize = 0;
        var index: usize = line.len - 1;
        while (index > 0) : (index -= 1) {
            const c = line[index];
            if (std.ascii.isDigit(c)) {
                // std.debug.print("found digit: {c}\n", .{c});
                right = index;
                break;
            }
        }
        // check digit in text
        for (character_numbers, 0..) |number, word_index| {
            const right_index = std.mem.lastIndexOf(u8, line, number);
            if (right_index != null and right_index.? > right) {
                // std.debug.print("found number: {s}\n", .{number});
                right = right_index.?;
                right_is_string = true;
                num[1] = transformToDigit(character_numbers[word_index]);
            }
        }

        // fuse numbers and save in list
        if (!left_is_string) {
            num[0] = line[left];
        }
        if (!right_is_string) {
            num[1] = line[right];
        }
        // std.debug.print("{d} <> {d}\n", .{ num[0], num[1] });
        const fused_number = try std.fmt.parseInt(u8, &num, 10);
        // std.debug.print("fused number: {d}\n", .{fused_number});
        try numbers.append(fused_number);
    }

    // write out numbers
    var sum: u64 = 0;
    for (numbers.items) |num| {
        // std.debug.print("number: {d}\n", .{num});
        sum += num;
    }

    // write output
    std.debug.print("sum: {d}\n", .{sum});
}

pub fn transformToDigit(word_number: []const u8) u8 {
    if (stringCompare(word_number, "zero")) return '0';
    if (stringCompare(word_number, "one")) return '1';
    if (stringCompare(word_number, "two")) return '2';
    if (stringCompare(word_number, "three")) return '3';
    if (stringCompare(word_number, "four")) return '4';
    if (stringCompare(word_number, "five")) return '5';
    if (stringCompare(word_number, "six")) return '6';
    if (stringCompare(word_number, "seven")) return '7';
    if (stringCompare(word_number, "eight")) return '8';
    if (stringCompare(word_number, "nine")) return '9';

    unreachable;
}

fn stringCompare(a: []const u8, b: []const u8) bool {
    if (a.len != b.len) return false;
    for (a, b) |ab, bb| {
        if (ab != bb) return false;
    }
    return true;
}

pub fn readInputFile(comptime path: []const u8, list: *std.ArrayList([]const u8)) !void {
    const file = try std.fs.cwd().openFile(path, .{ .mode = .read_only });
    defer file.close();

    const allocator = std.heap.page_allocator;
    // TODO: do I leak here?
    var reader = file.reader();
    var file_buffer: [1024]u8 = undefined;
    while (try reader.readUntilDelimiterOrEof(&file_buffer, '\n')) |line| {
        // std.debug.print("read index {s}\n", .{line});
        const l = try std.mem.Allocator.dupeZ(allocator, u8, line);
        try list.append(l);
    }
}
