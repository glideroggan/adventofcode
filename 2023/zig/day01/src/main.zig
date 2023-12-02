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
    var numbers = std.ArrayList(u64).init(arena.allocator());
    for (list.items) |line| {
        // process line
        // std.debug.print("index: {d}\n", .{index});
        // std.debug.print("line: {s}\n", .{line});
        // find number on left side
        var left: usize = 0;
        for (line, 0..) |c, index| {
            if (std.ascii.isDigit(c)) {
                // std.debug.print("found digit: {c}\n", .{c});
                left = index;
                break;
            }
        }
        var right: usize = 0;
        var index = line.len - 1;
        while (index >= 0) : (index -= 1) {
            const c = line[index];
            if (std.ascii.isDigit(c)) {
                // std.debug.print("found digit: {c}\n", .{c});
                right = index;
                break;
            }
        }

        // std.debug.print("{d} <> {d}\n", .{ left, right });
        // fuse numbers and save in list
        const num = [2]u8{ line[left], line[right] };
        const fused_number = try std.fmt.parseInt(u8, &num, 10);
        try numbers.append(fused_number);
    }

    // write out numbers
    var sum: u64 = 0;
    for (numbers.items) |num| {
        std.debug.print("number: {d}\n", .{num});
        sum += num;
    }

    // write output
    std.debug.print("sum: {d}\n", .{sum});
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
