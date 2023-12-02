const std = @import("std");
const expect = @import("std").testing.expect;

pub fn main() !void {
    // printing
    std.debug.print("Hello, {s}!\n", .{"World"});

    const name = [_]u8{ 'R', 'o', 'g', 'e', 'r' };
    if (name[0] == 'R') {
        std.debug.print("Your name starts with R!\n", .{});
    }
    std.debug.print("Hello, {s}!\n", .{name});

    // file

}
const FileOpenError = error{ AccessDenied, OutOfMemory, FileNotFound };
const AllocationError = error{OutOfMemory};

test "coerce error from a subset to a superset" {
    const err: FileOpenError = AllocationError.OutOfMemory;
    try expect(err == FileOpenError.OutOfMemory);
}

test "error union" {
    const maybe_error: AllocationError!u16 = 10;
    const no_error = maybe_error catch 0;

    try expect(@TypeOf(no_error) == u16);
    try expect(no_error == 10);
}

test "simple test" {
    var list = std.ArrayList(i32).init(std.testing.allocator);
    defer list.deinit(); // try commenting this out and see if zig detects the memory leak!
    try list.append(42);
    try std.testing.expectEqual(@as(i32, 42), list.pop());
}
